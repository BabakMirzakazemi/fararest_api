namespace Services.DTOs.Common;

public class PagingDTO<T> : PagingWithoutData where T : class
{
    public PagingDTO()
        : base()
    {
        Data = Array.Empty<T>();
    }

    public PagingDTO(IEnumerable<T> data, PagingRequest pagingRequest, long totalCount, string? url = null, string? pageParameterName = null, Dictionary<string, object>? otherValues = null) : base(pagingRequest, totalCount, url, pageParameterName, otherValues)
    {
        Data = data;
    }

    public IEnumerable<T> Data { get; set; }
}

public class PagingWithoutData
{
    public PagingWithoutData()
    {
    }

    public PagingWithoutData(PagingRequest pagingRequest, long totalCount, string? url = null, string? pageParameterName = null, Dictionary<string, object>? otherValues = null)
    {
        CurrentPage = pagingRequest.PageIndex;
        PageSize = pagingRequest.PageSize;
        TotalCount = totalCount;
        TotalPage = (int)Math.Ceiling(totalCount / (decimal)pagingRequest.PageSize);
        Url = url;
        PageParameterName = pageParameterName;
        OtherValues = otherValues;
    }

    public int CurrentPage { get; set; }

    public int PageSize { get; set; }

    public long TotalCount { get; set; }

    public int TotalPage { get; set; }

    public bool HasNext => TotalCount > PageSize;

    public bool HasPrevious => CurrentPage > 1;

    public string? Url { get; set; }

    public string? PageParameterName { get; set; }

    public Dictionary<string, object>? OtherValues { get; set; }
}
