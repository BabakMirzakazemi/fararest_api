namespace Services.DTOs.Admins.Get;
public sealed class GetAdminsPaginatedRequest : PagingRequest
{
    public string? Search { get; set; }
}
