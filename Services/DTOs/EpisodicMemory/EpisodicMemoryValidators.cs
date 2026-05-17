using Entities.EpisodicMemory;
using FluentValidation;

namespace Services.DTOs.EpisodicMemory;

public sealed class RecordEpisodeRequestValidator : AbstractValidator<RecordEpisodeRequest>
{
    public RecordEpisodeRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(200);

        RuleFor(x => x.Summary)
            .NotEmpty()
            .MinimumLength(16)
            .MaximumLength(1000);

        RuleFor(x => x.Details)
            .MaximumLength(20000)
            .When(x => !string.IsNullOrWhiteSpace(x.Details));

        RuleFor(x => x.OccurredAtUtc)
            .NotEqual(default(DateTimeOffset))
            .WithMessage("OccurredAtUtc is required.");

        RuleFor(x => x.ActorId)
            .MaximumLength(128)
            .When(x => !string.IsNullOrWhiteSpace(x.ActorId));

        RuleFor(x => x.ActorName)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.ActorName));

        RuleFor(x => x.CorrelationId)
            .MaximumLength(128)
            .When(x => !string.IsNullOrWhiteSpace(x.CorrelationId));

        RuleFor(x => x.Environment)
            .MaximumLength(64)
            .When(x => !string.IsNullOrWhiteSpace(x.Environment));

        RuleFor(x => x.CommitSha)
            .MaximumLength(64)
            .When(x => !string.IsNullOrWhiteSpace(x.CommitSha));

        RuleFor(x => x.DeduplicationKey)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.DeduplicationKey));

        RuleFor(x => x.MetadataJson)
            .MaximumLength(8000)
            .When(x => !string.IsNullOrWhiteSpace(x.MetadataJson));

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Details) || x.Tags.Count > 0 || x.References.Count > 0)
            .WithMessage("At least one of Details, Tags, or References must be provided.");

        RuleForEach(x => x.Tags)
            .NotEmpty()
            .MaximumLength(64);

        RuleForEach(x => x.References)
            .SetValidator(new EpisodeReferenceInputValidator());

        RuleFor(x => x.Tags.Count)
            .LessThanOrEqualTo(12)
            .WithMessage("No more than 12 tags should be provided for one episode.");

        RuleFor(x => x.References.Count)
            .LessThanOrEqualTo(12)
            .WithMessage("No more than 12 references should be provided for one episode.");

        RuleFor(x => x)
            .Must(HasEnoughStructuredContext)
            .WithMessage("High-value episodes should include at least one stable reference.");
    }

    private static bool HasEnoughStructuredContext(RecordEpisodeRequest request)
    {
        if (request.References.Count > 0)
            return true;

        if (request.Importance >= EpisodeImportance.High)
            return false;

        return request.Type is not (
            EpisodeType.ArchitecturalDecision or
            EpisodeType.BugFixed or
            EpisodeType.Migration or
            EpisodeType.FailedAttempt or
            EpisodeType.SuccessfulImplementation or
            EpisodeType.DependencyUpgrade or
            EpisodeType.DeploymentEvent or
            EpisodeType.PerformanceFinding or
            EpisodeType.SecurityFinding);
    }
}

public sealed class SearchEpisodesRequestValidator : AbstractValidator<SearchEpisodesRequest>
{
    public SearchEpisodesRequestValidator()
    {
        ApplyPagingRules();

        RuleFor(x => x.Query)
            .MaximumLength(300)
            .When(x => !string.IsNullOrWhiteSpace(x.Query));

        RuleFor(x => x.Environment)
            .MaximumLength(64)
            .When(x => !string.IsNullOrWhiteSpace(x.Environment));

        RuleFor(x => x.ReferenceKey)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.ReferenceKey));

        RuleFor(x => x.CommitSha)
            .MaximumLength(64)
            .When(x => !string.IsNullOrWhiteSpace(x.CommitSha));

        RuleFor(x => x.CorrelationId)
            .MaximumLength(128)
            .When(x => !string.IsNullOrWhiteSpace(x.CorrelationId));

        RuleFor(x => x.CandidatePoolSize)
            .InclusiveBetween(20, 400);

        RuleFor(x => x)
            .Must(x => !x.OccurredFromUtc.HasValue || !x.OccurredToUtc.HasValue || x.OccurredFromUtc <= x.OccurredToUtc)
            .WithMessage("OccurredFromUtc must be less than or equal to OccurredToUtc.");

        RuleForEach(x => x.Tags)
            .NotEmpty()
            .MaximumLength(64);

        RuleForEach(x => x.BoostTags)
            .NotEmpty()
            .MaximumLength(64);

        RuleForEach(x => x.BoostReferenceKeys)
            .NotEmpty()
            .MaximumLength(256);
    }

    private void ApplyPagingRules()
    {
        RuleFor(x => x.PageIndex).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class GetRecentEpisodesRequestValidator : AbstractValidator<GetRecentEpisodesRequest>
{
    public GetRecentEpisodesRequestValidator()
    {
        RuleFor(x => x.PageIndex).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Days).InclusiveBetween(1, 3650);
    }
}

public sealed class GetImportantEpisodesRequestValidator : AbstractValidator<GetImportantEpisodesRequest>
{
    public GetImportantEpisodesRequestValidator()
    {
        RuleFor(x => x.PageIndex).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Days).InclusiveBetween(1, 3650);
    }
}

public sealed class EpisodeReferenceInputValidator : AbstractValidator<EpisodeReferenceInput>
{
    public EpisodeReferenceInputValidator()
    {
        RuleFor(x => x.ReferenceKey)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.ReferenceLabel)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.ReferenceLabel));
    }
}

public sealed class EvaluateEpisodeSearchRequestValidator : AbstractValidator<EvaluateEpisodeSearchRequest>
{
    public EvaluateEpisodeSearchRequestValidator()
    {
        RuleFor(x => x.Search)
            .SetValidator(new SearchEpisodesRequestValidator());

        RuleFor(x => x.TopK)
            .InclusiveBetween(1, 50);

        RuleFor(x => x.ExpectedEpisodeIds.Count)
            .GreaterThan(0)
            .WithMessage("At least one expected episode id is required for evaluation.");
    }
}
