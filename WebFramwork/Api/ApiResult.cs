using Common.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Services.DTOs.Common;
using System.Net;

namespace WebFramework.Api;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiResult
{
    public ApiResult(bool isSuccess, HttpStatusCode statusCode, string? message = null, List<ValidationError>? validationErrors = null)
    {
        Meta = new MetaModel
        {
            Code = (int)statusCode,
            ErrorMessage = message ?? statusCode.ToDisplay(),
            IsSuccess = isSuccess,
            ErrorType = isSuccess ? null : statusCode.ToString(),
            ValidationErrors = validationErrors
        };
    }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public MetaModel Meta { get; }

    #region Implicit Operators
    public static implicit operator ApiResult(OkResult result)
    {
        return new ApiResult(true, HttpStatusCode.OK);
    }

    public static implicit operator ApiResult(BadRequestResult result)
    {
        return new ApiResult(false, HttpStatusCode.BadRequest);
    }

    public static implicit operator ApiResult(BadRequestObjectResult result)
    {
        var message = result.Value?.ToString();
        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            message = string.Join(" | ", errorMessages);
        }
        return new ApiResult(false, HttpStatusCode.BadRequest, message);
    }

    public static implicit operator ApiResult(ContentResult result)
    {
        return new ApiResult(true, HttpStatusCode.OK, result.Content);
    }

    public static implicit operator ApiResult(NotFoundResult result)
    {
        return new ApiResult(false, HttpStatusCode.NotFound);
    }
    #endregion
}

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ApiResult<TData> : ApiResult where TData : class
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TData? Data { get; set; }

    public ApiResult(bool isSuccess, HttpStatusCode statusCode, TData? data, string? message = null, List<ValidationError>? validationErrors = null)
        : base(isSuccess, statusCode, message, validationErrors)
    {
        Data = data;
    }

    public ApiResult(OperationResult<TData> operationResult, HttpStatusCode statusCode)
        : base(operationResult.IsSuccess, statusCode, operationResult.Message)
    {
        Data = operationResult.Data!;
    }

    #region Implicit Operators
    public static implicit operator ApiResult<TData>(TData data)
    {
        return new ApiResult<TData>(true, HttpStatusCode.OK, data);
    }

    public static implicit operator ApiResult<TData>(OkResult result)
    {
        return new ApiResult<TData>(true, HttpStatusCode.OK, null);
    }

    public static implicit operator ApiResult<TData>(OkObjectResult result)
    {
        return new ApiResult<TData>(true, HttpStatusCode.OK, CastToData(result.Value));
    }

    public static implicit operator ApiResult<TData>(BadRequestResult result)
    {
        return new ApiResult<TData>(false, HttpStatusCode.BadRequest, null);
    }

    public static implicit operator ApiResult<TData>(BadRequestObjectResult result)
    {
        var message = result.Value?.ToString();
        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            message = string.Join(" | ", errorMessages);
        }
        return new ApiResult<TData>(false, HttpStatusCode.BadRequest, null, message);
    }

    public static implicit operator ApiResult<TData>(ContentResult result)
    {
        return new ApiResult<TData>(true, HttpStatusCode.OK, null, result.Content);
    }

    public static implicit operator ApiResult<TData>(NotFoundResult result)
    {
        return new ApiResult<TData>(false, HttpStatusCode.NotFound, null);
    }

    public static implicit operator ApiResult<TData>(NotFoundObjectResult result)
    {
        return new ApiResult<TData>(false, HttpStatusCode.NotFound, CastToData(result.Value));
    }

    #endregion

    private static TData? CastToData(object? value)
    {
        return value is null ? null : (TData)value;
    }
}

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class MetaModel
{
    public bool IsSuccess { get; set; }

    public string? ErrorType { get; set; }

    public string? ErrorMessage { get; set; }

    public List<ValidationError>? ValidationErrors { get; set; }

    public int Code { get; set; }
}

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class ValidationError
{
    public string Property { get; set; } = null!;

    public string Message { get; set; } = null!;
}
