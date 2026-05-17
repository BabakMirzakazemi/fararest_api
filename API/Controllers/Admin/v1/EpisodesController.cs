using Entities.Users;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts.EpisodicMemory;
using Services.DTOs.Common;
using Services.DTOs.EpisodicMemory;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.Admin.v1;

[ApiVersion("1")]
public sealed class EpisodesController(IEpisodicMemoryService episodicMemoryService) : BaseAdminApiController
{
    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<EpisodeDto> RecordAsync(RecordEpisodeRequest request, CancellationToken cancellationToken)
        => await episodicMemoryService.RecordEpisodeAsync(request, cancellationToken);

    [HttpGet("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<EpisodeDto> GetAsync([FromQuery] Guid id, CancellationToken cancellationToken)
        => await episodicMemoryService.GetEpisodeAsync(id, cancellationToken);

    [HttpPost("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<PagingDTO<EpisodeListItemDto>> SearchAsync(SearchEpisodesRequest request, CancellationToken cancellationToken)
        => await episodicMemoryService.SearchEpisodesAsync(request, cancellationToken);

    [HttpGet("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<PagingDTO<EpisodeListItemDto>> RecentAsync([FromQuery] GetRecentEpisodesRequest request, CancellationToken cancellationToken)
        => await episodicMemoryService.GetRecentEpisodesAsync(request, cancellationToken);

    [HttpGet("[action]")]
    [ApiCustomAuthorize(false, RoleHelper.SuperAdmin, RoleHelper.Admin)]
    public async Task<PagingDTO<EpisodeListItemDto>> ImportantAsync([FromQuery] GetImportantEpisodesRequest request, CancellationToken cancellationToken)
        => await episodicMemoryService.GetImportantEpisodesAsync(request, cancellationToken);
}
