using Services.DTOs.EpisodicMemory;

namespace Services.Contracts.EpisodicMemory;

public interface IEpisodicMemoryService
{
    Task<EpisodeDto> RecordEpisodeAsync(RecordEpisodeRequest request, CancellationToken cancellationToken);
    Task<EpisodeDto> GetEpisodeAsync(Guid id, CancellationToken cancellationToken);
    Task<PagingDTO<EpisodeListItemDto>> SearchEpisodesAsync(SearchEpisodesRequest request, CancellationToken cancellationToken);
    Task<EpisodeSearchEvaluationDto> EvaluateSearchAsync(EvaluateEpisodeSearchRequest request, CancellationToken cancellationToken);
    Task<PagingDTO<EpisodeListItemDto>> GetRecentEpisodesAsync(GetRecentEpisodesRequest request, CancellationToken cancellationToken);
    Task<PagingDTO<EpisodeListItemDto>> GetImportantEpisodesAsync(GetImportantEpisodesRequest request, CancellationToken cancellationToken);
}
