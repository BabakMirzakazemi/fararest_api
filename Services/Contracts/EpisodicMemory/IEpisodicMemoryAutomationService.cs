using Services.DTOs.EpisodicMemory;

namespace Services.Contracts.EpisodicMemory;

public interface IEpisodicMemoryAutomationService
{
    Task TryRecordAsync(RecordEpisodeRequest request, CancellationToken cancellationToken);
}
