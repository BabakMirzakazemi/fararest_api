using Common.Markers;
using Entities.EpisodicMemory;
using Services.Contracts.EpisodicMemory;
using Services.Contracts.Repositories;
using Services.DTOs.EpisodicMemory;

namespace Services.Services.EpisodicMemory;

public sealed class EpisodicMemoryService(IRepository<Episode> episodeRepository) : IEpisodicMemoryService, IScopedDependency
{
    private const int DuplicateWindowDays = 30;
    private const int MaxHybridCandidatePoolSize = 400;

    public async Task<EpisodeDto> RecordEpisodeAsync(RecordEpisodeRequest request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        if (request.OccurredAtUtc > now.AddMinutes(5))
            throw new BadRequestException("OccurredAtUtc cannot be in the future.");

        if (request.ParentEpisodeId.HasValue)
        {
            var parentExists = await episodeRepository.IsExistAsync(x => x.Id == request.ParentEpisodeId.Value, cancellationToken);
            if (!parentExists)
                throw new NotFoundException($"Parent episode '{request.ParentEpisodeId}' was not found.");
        }

        var normalizedTags = NormalizeTags(request.Tags);
        var normalizedReferences = NormalizeReferences(request.References);
        var deduplicationKey = BuildDeduplicationKey(request, normalizedReferences);

        var duplicate = await episodeRepository.TableNoTracking
            .TagWith("Episodes.Record.Deduplicate")
            .Where(x => x.DeduplicationKey == deduplicationKey && x.RecordedAtUtc >= now.AddDays(-DuplicateWindowDays))
            .OrderByDescending(x => x.RecordedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (duplicate != null)
            throw new BadRequestException($"A similar episode already exists with id '{duplicate.Id}'.");

        var entity = new Episode
        {
            Type = request.Type,
            Importance = request.Importance,
            Source = request.Source,
            Status = request.Status,
            Title = request.Title.Trim(),
            Summary = request.Summary.Trim(),
            Details = string.IsNullOrWhiteSpace(request.Details) ? null : request.Details.Trim(),
            OccurredAtUtc = request.OccurredAtUtc,
            RecordedAtUtc = now,
            ActorId = NormalizeNullable(request.ActorId),
            ActorName = NormalizeNullable(request.ActorName),
            CorrelationId = NormalizeNullable(request.CorrelationId),
            Environment = NormalizeNullable(request.Environment),
            CommitSha = NormalizeNullable(request.CommitSha),
            DeduplicationKey = deduplicationKey,
            MetadataJson = NormalizeNullable(request.MetadataJson),
            ParentEpisodeId = request.ParentEpisodeId,
            Tags = normalizedTags.Select(tag => new EpisodeTag
            {
                Tag = tag
            }).ToList(),
            References = normalizedReferences.Select(reference => new EpisodeReference
            {
                Type = reference.Type,
                ReferenceKey = reference.ReferenceKey,
                ReferenceLabel = reference.ReferenceLabel
            }).ToList()
        };

        await episodeRepository.AddAsync(entity, cancellationToken);
        return await GetEpisodeAsync(entity.Id, cancellationToken);
    }

    public async Task<EpisodeDto> GetEpisodeAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await episodeRepository.TableNoTracking
            .TagWith("Episodes.Get")
            .Include(x => x.Tags)
            .Include(x => x.References)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Episode '{id}' was not found.");

        return MapEpisode(entity);
    }

    public async Task<PagingDTO<EpisodeListItemDto>> SearchEpisodesAsync(SearchEpisodesRequest request, CancellationToken cancellationToken)
    {
        IQueryable<Episode> query = episodeRepository.TableNoTracking
            .TagWith("Episodes.Search");

        query = ApplySearchFilters(query, request);
        var totalCount = await query.CountAsync(cancellationToken);
        IReadOnlyList<EpisodeListItemDto> page = ShouldUseHybridRanking(request)
            ? await ExecuteHybridSearchAsync(query, request, cancellationToken)
            : await ExecuteSimpleSearchAsync(query, request, cancellationToken);

        return new PagingDTO<EpisodeListItemDto>(page, request, totalCount);
    }

    public async Task<EpisodeSearchEvaluationDto> EvaluateSearchAsync(EvaluateEpisodeSearchRequest request, CancellationToken cancellationToken)
    {
        var normalizedSearch = request.Search;
        normalizedSearch.PageIndex = 1;
        normalizedSearch.PageSize = Math.Max(normalizedSearch.PageSize, request.TopK);
        normalizedSearch.CandidatePoolSize = Math.Max(normalizedSearch.CandidatePoolSize, request.TopK * 5);
        normalizedSearch.UseHybridRanking = true;

        var searchResults = await SearchEpisodesAsync(normalizedSearch, cancellationToken);
        var topResults = searchResults.Data
            .Take(request.TopK)
            .ToList();

        var expectedIds = request.ExpectedEpisodeIds
            .Distinct()
            .ToHashSet();

        var matchedResults = topResults
            .Where(x => expectedIds.Contains(x.Id))
            .ToList();

        var firstRelevantRank = topResults
            .Select((item, index) => new { item.Id, Rank = index + 1 })
            .FirstOrDefault(x => expectedIds.Contains(x.Id))
            ?.Rank;

        var hitCount = matchedResults.Count;
        var precisionAtK = topResults.Count == 0 ? 0d : hitCount / (double)topResults.Count;
        var recallAtK = expectedIds.Count == 0 ? 0d : hitCount / (double)expectedIds.Count;
        var reciprocalRank = firstRelevantRank.HasValue ? 1d / firstRelevantRank.Value : 0d;

        return new EpisodeSearchEvaluationDto
        {
            TopK = request.TopK,
            ResultCount = topResults.Count,
            ExpectedCount = expectedIds.Count,
            HitCount = hitCount,
            RecallAtK = recallAtK,
            PrecisionAtK = precisionAtK,
            FirstRelevantRank = firstRelevantRank,
            ReciprocalRank = reciprocalRank,
            MatchedEpisodeIds = matchedResults.Select(x => x.Id).ToList(),
            TopResults = topResults
                .Select((item, index) => new EpisodeSearchEvaluationResultDto
                {
                    Rank = index + 1,
                    EpisodeId = item.Id,
                    Title = item.Title,
                    RetrievalScore = item.RetrievalScore,
                    IsExpectedHit = expectedIds.Contains(item.Id)
                })
                .ToList()
        };
    }

    public Task<PagingDTO<EpisodeListItemDto>> GetRecentEpisodesAsync(GetRecentEpisodesRequest request, CancellationToken cancellationToken)
    {
        var searchRequest = new SearchEpisodesRequest
        {
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            Types = request.Types,
            OccurredFromUtc = DateTimeOffset.UtcNow.AddDays(-request.Days)
        };

        return SearchEpisodesAsync(searchRequest, cancellationToken);
    }

    public Task<PagingDTO<EpisodeListItemDto>> GetImportantEpisodesAsync(GetImportantEpisodesRequest request, CancellationToken cancellationToken)
    {
        var importances = Enum.GetValues<EpisodeImportance>()
            .Where(x => x >= request.MinimumImportance)
            .ToArray();

        var searchRequest = new SearchEpisodesRequest
        {
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            Importances = importances,
            OccurredFromUtc = DateTimeOffset.UtcNow.AddDays(-request.Days)
        };

        return SearchEpisodesAsync(searchRequest, cancellationToken);
    }

    private static IQueryable<Episode> ApplySearchFilters(IQueryable<Episode> query, SearchEpisodesRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var queryText = request.Query.Trim();
            query = query.Where(x =>
                x.Title.Contains(queryText) ||
                x.Summary.Contains(queryText) ||
                (x.Details != null && x.Details.Contains(queryText)));
        }

        if (request.Types.Count > 0)
            query = query.Where(x => request.Types.Contains(x.Type));

        if (request.Importances.Count > 0)
            query = query.Where(x => request.Importances.Contains(x.Importance));

        if (request.Sources.Count > 0)
            query = query.Where(x => request.Sources.Contains(x.Source));

        if (!string.IsNullOrWhiteSpace(request.Environment))
        {
            var environment = request.Environment.Trim();
            query = query.Where(x => x.Environment == environment);
        }

        if (request.OccurredFromUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc >= request.OccurredFromUtc.Value);

        if (request.OccurredToUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc <= request.OccurredToUtc.Value);

        if (request.Tags.Count > 0)
        {
            var tags = NormalizeTags(request.Tags);
            query = query.Where(x => x.Tags.Any(tag => tags.Contains(tag.Tag)));
        }

        if (request.ReferenceType.HasValue)
            query = query.Where(x => x.References.Any(reference => reference.Type == request.ReferenceType.Value));

        if (!string.IsNullOrWhiteSpace(request.ReferenceKey))
        {
            var referenceKey = request.ReferenceKey.Trim();
            query = query.Where(x => x.References.Any(reference => reference.ReferenceKey == referenceKey));
        }

        if (!string.IsNullOrWhiteSpace(request.CommitSha))
        {
            var commitSha = request.CommitSha.Trim();
            query = query.Where(x => x.CommitSha == commitSha);
        }

        if (!string.IsNullOrWhiteSpace(request.CorrelationId))
        {
            var correlationId = request.CorrelationId.Trim();
            query = query.Where(x => x.CorrelationId == correlationId);
        }

        return query;
    }

    private static bool ShouldUseHybridRanking(SearchEpisodesRequest request)
    {
        return request.UseHybridRanking &&
            (!string.IsNullOrWhiteSpace(request.Query) ||
             request.BoostTags.Count > 0 ||
             request.BoostReferenceKeys.Count > 0 ||
             request.PreferRecent ||
             request.PreferImportant);
    }

    private static async Task<IReadOnlyList<EpisodeListItemDto>> ExecuteSimpleSearchAsync(
        IQueryable<Episode> query,
        SearchEpisodesRequest request,
        CancellationToken cancellationToken)
    {
        return await query
            .OrderByDescending(x => x.OccurredAtUtc)
            .ThenByDescending(x => x.RecordedAtUtc)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new EpisodeListItemDto
            {
                Id = x.Id,
                Type = x.Type,
                Importance = x.Importance,
                Source = x.Source,
                Status = x.Status,
                Title = x.Title,
                Summary = x.Summary,
                OccurredAtUtc = x.OccurredAtUtc,
                RecordedAtUtc = x.RecordedAtUtc,
                ActorName = x.ActorName,
                Environment = x.Environment,
                CommitSha = x.CommitSha,
                Tags = x.Tags.OrderBy(t => t.Tag).Select(t => t.Tag).ToList()
            })
            .ToListAsync(cancellationToken);
    }

    private static async Task<IReadOnlyList<EpisodeListItemDto>> ExecuteHybridSearchAsync(
        IQueryable<Episode> query,
        SearchEpisodesRequest request,
        CancellationToken cancellationToken)
    {
        var candidatePoolSize = Math.Clamp(
            Math.Max(request.CandidatePoolSize, request.PageIndex * request.PageSize * 5),
            request.PageSize,
            MaxHybridCandidatePoolSize);

        var candidates = await query
            .Include(x => x.Tags)
            .Include(x => x.References)
            .OrderByDescending(x => x.OccurredAtUtc)
            .ThenByDescending(x => x.RecordedAtUtc)
            .Take(candidatePoolSize)
            .ToListAsync(cancellationToken);

        var ranked = candidates
            .Select(x => BuildRankedResult(x, request))
            .OrderByDescending(x => x.RetrievalScore ?? 0d)
            .ThenByDescending(x => x.Importance)
            .ThenByDescending(x => x.OccurredAtUtc)
            .ThenByDescending(x => x.RecordedAtUtc)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return ranked;
    }

    private static EpisodeListItemDto BuildRankedResult(Episode entity, SearchEpisodesRequest request)
    {
        var score = 0d;
        var signals = new List<string>();
        var normalizedBoostTags = NormalizeTags(request.BoostTags);
        var normalizedFilterTags = NormalizeTags(request.Tags);
        var normalizedBoostReferenceKeys = request.BoostReferenceKeys
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToHashSet(StringComparer.Ordinal);

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var queryText = request.Query.Trim();
            var tokens = queryText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            score += ScoreText(entity.Title, queryText, tokens, 14d, 5d, ref signals, "title");
            score += ScoreText(entity.Summary, queryText, tokens, 10d, 3d, ref signals, "summary");
            if (!string.IsNullOrWhiteSpace(entity.Details))
                score += ScoreText(entity.Details, queryText, tokens, 4d, 1.2d, ref signals, "details");
        }

        if (normalizedFilterTags.Count > 0)
        {
            var tagHits = entity.Tags.Count(x => normalizedFilterTags.Contains(x.Tag));
            if (tagHits > 0)
            {
                score += tagHits * 1.5d;
                signals.Add($"tag-filter:{tagHits}");
            }
        }

        if (normalizedBoostTags.Count > 0)
        {
            var tagHits = entity.Tags.Count(x => normalizedBoostTags.Contains(x.Tag));
            if (tagHits > 0)
            {
                score += tagHits * 4d;
                signals.Add($"tag-boost:{tagHits}");
            }
        }

        if (!string.IsNullOrWhiteSpace(request.ReferenceKey))
        {
            var exactReferenceHit = entity.References.Any(x => x.ReferenceKey == request.ReferenceKey.Trim());
            if (exactReferenceHit)
            {
                score += 5d;
                signals.Add("reference-filter");
            }
        }

        if (normalizedBoostReferenceKeys.Count > 0)
        {
            var referenceHits = entity.References.Count(x => normalizedBoostReferenceKeys.Contains(x.ReferenceKey));
            if (referenceHits > 0)
            {
                score += referenceHits * 6d;
                signals.Add($"reference-boost:{referenceHits}");
            }
        }

        if (request.PreferImportant)
        {
            var importanceBoost = (int)entity.Importance * 1.75d;
            score += importanceBoost;
            signals.Add($"importance:{entity.Importance}");
        }

        if (request.PreferRecent)
        {
            var ageDays = Math.Max(0d, (DateTimeOffset.UtcNow - entity.RecordedAtUtc).TotalDays);
            var recentBoost = Math.Max(0d, 12d - (ageDays / 7d));
            if (recentBoost > 0d)
            {
                score += recentBoost;
                signals.Add("recent");
            }
        }

        return new EpisodeListItemDto
        {
            Id = entity.Id,
            Type = entity.Type,
            Importance = entity.Importance,
            Source = entity.Source,
            Status = entity.Status,
            Title = entity.Title,
            Summary = entity.Summary,
            OccurredAtUtc = entity.OccurredAtUtc,
            RecordedAtUtc = entity.RecordedAtUtc,
            ActorName = entity.ActorName,
            Environment = entity.Environment,
            CommitSha = entity.CommitSha,
            Tags = entity.Tags.OrderBy(t => t.Tag).Select(t => t.Tag).ToList(),
            RetrievalScore = Math.Round(score, 2),
            RetrievalSignals = signals.Distinct(StringComparer.Ordinal).ToList()
        };
    }

    private static double ScoreText(
        string? text,
        string queryText,
        IReadOnlyList<string> tokens,
        double exactMatchWeight,
        double tokenMatchWeight,
        ref List<string> signals,
        string signalPrefix)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0d;

        var score = 0d;
        if (text.Contains(queryText, StringComparison.OrdinalIgnoreCase))
        {
            score += exactMatchWeight;
            signals.Add($"{signalPrefix}-exact");
        }

        var tokenHits = tokens.Count(token => text.Contains(token, StringComparison.OrdinalIgnoreCase));
        if (tokenHits > 0)
        {
            score += tokenHits * tokenMatchWeight;
            signals.Add($"{signalPrefix}-tokens:{tokenHits}");
        }

        return score;
    }

    private static EpisodeDto MapEpisode(Episode entity)
    {
        return new EpisodeDto
        {
            Id = entity.Id,
            Type = entity.Type,
            Importance = entity.Importance,
            Source = entity.Source,
            Status = entity.Status,
            Title = entity.Title,
            Summary = entity.Summary,
            Details = entity.Details,
            OccurredAtUtc = entity.OccurredAtUtc,
            RecordedAtUtc = entity.RecordedAtUtc,
            ActorId = entity.ActorId,
            ActorName = entity.ActorName,
            CorrelationId = entity.CorrelationId,
            Environment = entity.Environment,
            CommitSha = entity.CommitSha,
            DeduplicationKey = entity.DeduplicationKey,
            MetadataJson = entity.MetadataJson,
            ParentEpisodeId = entity.ParentEpisodeId,
            Tags = entity.Tags.OrderBy(x => x.Tag).Select(x => x.Tag).ToList(),
            References = entity.References
                .OrderBy(x => x.Type)
                .ThenBy(x => x.ReferenceKey)
                .Select(x => new EpisodeReferenceDto
                {
                    Type = x.Type,
                    ReferenceKey = x.ReferenceKey,
                    ReferenceLabel = x.ReferenceLabel
                })
                .ToList()
        };
    }

    private static List<string> NormalizeTags(IEnumerable<string> tags)
    {
        return tags
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToLowerInvariant())
            .Distinct(StringComparer.Ordinal)
            .ToList();
    }

    private static List<EpisodeReferenceInput> NormalizeReferences(IEnumerable<EpisodeReferenceInput> references)
    {
        return references
            .Where(x => !string.IsNullOrWhiteSpace(x.ReferenceKey))
            .Select(x => new EpisodeReferenceInput
            {
                Type = x.Type,
                ReferenceKey = x.ReferenceKey.Trim(),
                ReferenceLabel = NormalizeNullable(x.ReferenceLabel)
            })
            .GroupBy(x => new { x.Type, x.ReferenceKey })
            .Select(group => group.First())
            .ToList();
    }

    private static string BuildDeduplicationKey(RecordEpisodeRequest request, IReadOnlyList<EpisodeReferenceInput> references)
    {
        if (!string.IsNullOrWhiteSpace(request.DeduplicationKey))
            return request.DeduplicationKey.Trim().ToLowerInvariant();

        var normalizedTitle = request.Title.Trim().ToLowerInvariant();
        var referencePart = references.Count == 0
            ? "no-ref"
            : $"{references[0].Type}:{references[0].ReferenceKey}".ToLowerInvariant();

        return $"{request.Type}|{normalizedTitle}|{referencePart}|{request.OccurredAtUtc.UtcDateTime:yyyyMMdd}";
    }

    private static string? NormalizeNullable(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
