using Common.Markers;
using Microsoft.Extensions.Logging;
using Services.Contracts.EpisodicMemory;
using Services.DTOs.EpisodicMemory;

namespace Services.Services.EpisodicMemory;

public sealed class EpisodicMemoryAutomationService(
    IEpisodicMemoryService episodicMemoryService,
    ILogger<EpisodicMemoryAutomationService> logger) : IEpisodicMemoryAutomationService, IScopedDependency
{
    public async Task TryRecordAsync(RecordEpisodeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await episodicMemoryService.RecordEpisodeAsync(request, cancellationToken);
        }
        catch (BadRequestException exception)
        {
            logger.LogInformation(exception, "Skipped automated episodic memory recording for '{Title}'.", request.Title);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Automated episodic memory recording failed for '{Title}'.", request.Title);
        }
    }
}
