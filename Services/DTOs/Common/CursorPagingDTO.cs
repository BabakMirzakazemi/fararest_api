namespace Services.DTOs.Common;

// Response contract for keyset/cursor pagination.
// NextCursor should be sent back as request cursor to fetch the next page.
public class CursorPagingDTO<T> where T : class
{
    public CursorPagingDTO()
    {
        Data = Array.Empty<T>();
    }

    public CursorPagingDTO(IEnumerable<T> data, int pageSize, bool hasNext, string? nextCursor, string? currentCursor = null)
    {
        Data = data;
        PageSize = pageSize;
        HasNext = hasNext;
        NextCursor = nextCursor;
        CurrentCursor = currentCursor;
    }

    public IEnumerable<T> Data { get; set; }

    public int PageSize { get; set; }

    public bool HasNext { get; set; }

    public string? NextCursor { get; set; }

    public string? CurrentCursor { get; set; }
}
