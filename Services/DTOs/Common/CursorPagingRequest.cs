namespace Services.DTOs.Common;

// Base request contract for keyset/cursor pagination.
// Cursor contains an opaque token created by the server from the last item of previous page.
public class CursorPagingRequest
{
    public int PageSize { get; set; } = 10;

    public string? Cursor { get; set; }
}

