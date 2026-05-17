using Entities.Users;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts.AgentTools;
using Services.DTOs.AgentTools;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.Admin.v1;

[ApiVersion("1")]
public sealed class AgentToolsController(IAgentToolingService agentToolingService) : BaseAdminApiController
{
    [HttpGet("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<AgentCapabilitiesDto> CapabilitiesAsync(CancellationToken cancellationToken)
        => await agentToolingService.GetCapabilitiesAsync(cancellationToken);

    [HttpGet("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<AgentDatabaseStatusDto> DatabaseStatusAsync(CancellationToken cancellationToken)
        => await agentToolingService.GetDatabaseStatusAsync(cancellationToken);

    [HttpGet("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<AgentMemoryStatusDto> MemoryStatusAsync(CancellationToken cancellationToken)
        => await agentToolingService.GetMemoryStatusAsync(cancellationToken);
}
