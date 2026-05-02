namespace Services.DTOs.Users;

public class FilterUserCursorRequest : CursorPagingRequest
{
    public string SearchText { get; set; } = string.Empty;
}

