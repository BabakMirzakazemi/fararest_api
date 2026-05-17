using Common.Markers;
using Entities.EpisodicMemory;
using Services.Contracts.EpisodicMemory;
using Services.Contracts.Repositories;
using Services.DTOs.EpisodicMemory;

namespace Services.Services.EpisodicMemory;

public sealed class EpisodicMemoryService(IRepository<Episode> episodeRepository) : IEpisodicMemoryService, IScopedDependency
{
    private const int DuplicateWindowDays = 30;

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
        var page = await query
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

        return new PagingDTO<EpisodeListItemDto>(page, request, totalCount);
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
