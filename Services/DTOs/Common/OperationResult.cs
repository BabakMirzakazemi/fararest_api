namespace Services.DTOs.Common;

/// <summary>
/// Used To Have Formatted Result In Methods
/// </summary>
public class OperationResult
{
    public bool IsSuccess { get; private set; }

    public string? Message { get; private set; }

    public OperationResult SetProperties(bool isSuccess, string? message = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        return this;
    }
}

/// <summary>
/// Used To Have Formatted Result In Methods, Have Generic Input For Different Result Types
/// </summary>
/// <typeparam name="TData">Return Type Of Method</typeparam>
public class OperationResult<TData>
{
    public TData? Data { get; private set; }

    public bool IsSuccess { get; private set; }

    public string? Message { get; private set; }

    /// <summary>
    /// Use This Method To Set Result Status
    /// </summary>
    /// <param name="data">If Method Has Return Type Pass It As Data</param>
    /// <param name="isSuccess">Operation Result Status</param>
    /// <param name="message">Operation Optional Message</param>
    public OperationResult<TData> SetProperties(TData data, bool isSuccess, string? message = null)
    {
        Data = data;
        IsSuccess = isSuccess;
        Message = message;
        return this;
    }
}
