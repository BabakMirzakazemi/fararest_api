using Services.DTOs.AgentTools;

namespace Services.Contracts.AgentTools;

public interface IAgentToolingService
{
    Task<AgentCapabilitiesDto> GetCapabilitiesAsync(CancellationToken cancellationToken);
    Task<AgentDatabaseStatusDto> GetDatabaseStatusAsync(CancellationToken cancellationToken);
    Task<AgentMemoryStatusDto> GetMemoryStatusAsync(CancellationToken cancellationToken);
}
