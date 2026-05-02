using Entities.Enums.Users;

namespace Services.DTOs.Users;

public class FilterUserRequest : PagingRequest
{
    public string SearchText { get; set; } = "";
}
