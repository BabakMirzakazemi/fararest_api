using Common.Exceptions;
using Entities.EpisodicMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Services.Contracts.EpisodicMemory;
using Services.DTOs.EpisodicMemory;
using System.Globalization;
using System.Net;
using WebFramework.Api;

namespace WebFramework.Middlewares;

public class CustomExceptionHandlerMiddleware(RequestDelegate next,
    IWebHostEnvironment env,
    ILogger<CustomExceptionHandlerMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly IWebHostEnvironment _env = env;
    private readonly ILogger<CustomExceptionHandlerMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        string? message = null;
        HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;

        try
        {
            await _next(context);
        }
        catch (AppException exception)
        {
            _logger.LogError(exception, exception.Message);
            await TryRecordAppExceptionEpisodeAsync(context, exception);
            httpStatusCode = exception.HttpStatusCode;
            message = exception.Message;
            await WriteToResponseAsync();
        }
        catch (SecurityTokenExpiredException exception)
        {
            _logger.LogError(exception, exception.Message);
            SetUnAuthorizeResponse(exception);
            await WriteToResponseAsync();
        }
        catch (UnauthorizedAccessException exception)
        {
            _logger.LogError(exception, exception.Message);
            SetUnAuthorizeResponse(exception);
            await WriteToResponseAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            await TryRecordUnhandledExceptionEpisodeAsync(context, exception);

            if (_env.IsDevelopment())
            {
                Dictionary<string, string?> dic = new Dictionary<string, string?>
                {
                    ["Exception"] = exception.Message,
                    ["StackTrace"] = exception.StackTrace,
                };
                message = JsonConvert.SerializeObject(dic);
            }
            await WriteToResponseAsync();
        }

        async Task WriteToResponseAsync()
        {
            if (context.Response.HasStarted)
                throw new InvalidOperationException("The response has already started, the http status code middleware will not be executed.");

            var result = new ApiResult(false, httpStatusCode, message);
            var json = JsonConvert.SerializeObject(result);

            context.Response.StatusCode = (int)httpStatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }

        void SetUnAuthorizeResponse(Exception exception)
        {
            httpStatusCode = HttpStatusCode.Unauthorized;

            if (_env.IsDevelopment())
            {
                Dictionary<string, string?> dic = new Dictionary<string, string?>
                {
                    ["Exception"] = exception.Message,
                    ["StackTrace"] = exception.StackTrace
                };
                if (exception is SecurityTokenExpiredException tokenException)
                    dic.Add("Expires", tokenException.Expires.ToString());

                message = JsonConvert.SerializeObject(dic);
            }
        }

        async Task TryRecordUnhandledExceptionEpisodeAsync(HttpContext httpContext, Exception exception)
        {
            if (httpContext.RequestServices.GetService(typeof(IEpisodicMemoryAutomationService)) is not IEpisodicMemoryAutomationService automationService)
                return;

            var endpointDisplayName = httpContext.GetEndpoint()?.DisplayName;
            var correlationId = httpContext.Items.TryGetValue(RequestContextEnrichmentMiddleware.CorrelationIdItemKey, out var rawCorrelationId)
                ? rawCorrelationId?.ToString()
                : httpContext.TraceIdentifier;
            var occurredAtUtc = DateTimeOffset.UtcNow;
            var routeReference = string.IsNullOrWhiteSpace(endpointDisplayName)
                ? $"{httpContext.Request.Method} {httpContext.Request.Path}"
                : endpointDisplayName;

            await automationService.TryRecordAsync(new RecordEpisodeRequest
            {
                Type = EpisodeType.Incident,
                Importance = EpisodeImportance.High,
                Source = EpisodeSource.System,
                Status = EpisodeStatus.Open,
                Title = $"Unhandled exception on {httpContext.Request.Method} {httpContext.Request.Path}",
                Summary = exception.Message,
                Details = BuildExceptionDetails(httpContext, exception, endpointDisplayName, correlationId),
                OccurredAtUtc = occurredAtUtc,
                ActorName = nameof(CustomExceptionHandlerMiddleware),
                CorrelationId = correlationId,
                Environment = _env.EnvironmentName,
                DeduplicationKey = $"incident|{exception.GetType().Name}|{httpContext.Request.Path.Value?.ToLowerInvariant()}|{occurredAtUtc:yyyyMMdd}",
                Tags =
                [
                    "incident",
                    "api",
                    "unhandled-exception",
                    _env.EnvironmentName.ToLowerInvariant()
                ],
                References =
                [
                    new EpisodeReferenceInput
                    {
                        Type = EpisodeReferenceType.ApiEndpoint,
                        ReferenceKey = routeReference,
                        ReferenceLabel = $"{httpContext.Request.Method} {httpContext.Request.Path}"
                    },
                    new EpisodeReferenceInput
                    {
                        Type = EpisodeReferenceType.Module,
                        ReferenceKey = "WebFramework.Middlewares.CustomExceptionHandlerMiddleware",
                        ReferenceLabel = nameof(CustomExceptionHandlerMiddleware)
                    }
                ]
            }, CancellationToken.None);
        }

        async Task TryRecordAppExceptionEpisodeAsync(HttpContext httpContext, AppException exception)
        {
            var statusCode = (int)exception.HttpStatusCode;
            if (statusCode < StatusCodes.Status500InternalServerError)
                return;

            if (httpContext.RequestServices.GetService(typeof(IEpisodicMemoryAutomationService)) is not IEpisodicMemoryAutomationService automationService)
                return;

            var endpointDisplayName = httpContext.GetEndpoint()?.DisplayName;
            var correlationId = httpContext.Items.TryGetValue(RequestContextEnrichmentMiddleware.CorrelationIdItemKey, out var rawCorrelationId)
                ? rawCorrelationId?.ToString()
                : httpContext.TraceIdentifier;
            var occurredAtUtc = DateTimeOffset.UtcNow;
            var routeReference = string.IsNullOrWhiteSpace(endpointDisplayName)
                ? $"{httpContext.Request.Method} {httpContext.Request.Path}"
                : endpointDisplayName;

            await automationService.TryRecordAsync(new RecordEpisodeRequest
            {
                Type = EpisodeType.Incident,
                Importance = EpisodeImportance.High,
                Source = EpisodeSource.System,
                Status = EpisodeStatus.Open,
                Title = $"AppException {statusCode} on {httpContext.Request.Method} {httpContext.Request.Path}",
                Summary = exception.Message ?? $"Unhandled AppException with status code {statusCode}.",
                Details = BuildAppExceptionDetails(httpContext, exception, endpointDisplayName, correlationId),
                OccurredAtUtc = occurredAtUtc,
                ActorName = nameof(CustomExceptionHandlerMiddleware),
                CorrelationId = correlationId,
                Environment = _env.EnvironmentName,
                DeduplicationKey = $"app-exception|{statusCode.ToString(CultureInfo.InvariantCulture)}|{httpContext.Request.Path.Value?.ToLowerInvariant()}|{occurredAtUtc:yyyyMMdd}",
                Tags =
                [
                    "incident",
                    "api",
                    "app-exception",
                    $"http-{statusCode}",
                    _env.EnvironmentName.ToLowerInvariant()
                ],
                References =
                [
                    new EpisodeReferenceInput
                    {
                        Type = EpisodeReferenceType.ApiEndpoint,
                        ReferenceKey = routeReference,
                        ReferenceLabel = $"{httpContext.Request.Method} {httpContext.Request.Path}"
                    },
                    new EpisodeReferenceInput
                    {
                        Type = EpisodeReferenceType.Module,
                        ReferenceKey = "WebFramework.Middlewares.CustomExceptionHandlerMiddleware",
                        ReferenceLabel = nameof(CustomExceptionHandlerMiddleware)
                    }
                ]
            }, CancellationToken.None);
        }
    }

    private static string BuildExceptionDetails(HttpContext context, Exception exception, string? endpointDisplayName, string? correlationId)
    {
        var details = new Dictionary<string, string?>
        {
            ["ExceptionType"] = exception.GetType().FullName,
            ["Message"] = exception.Message,
            ["Method"] = context.Request.Method,
            ["Path"] = context.Request.Path,
            ["QueryString"] = context.Request.QueryString.Value,
            ["Endpoint"] = endpointDisplayName,
            ["TraceIdentifier"] = context.TraceIdentifier,
            ["CorrelationId"] = correlationId,
            ["StackTrace"] = exception.StackTrace
        };

        return JsonConvert.SerializeObject(details);
    }

    private static string BuildAppExceptionDetails(HttpContext context, AppException exception, string? endpointDisplayName, string? correlationId)
    {
        var details = new Dictionary<string, string?>
        {
            ["ExceptionType"] = exception.GetType().FullName,
            ["Message"] = exception.Message,
            ["StatusCode"] = ((int)exception.HttpStatusCode).ToString(CultureInfo.InvariantCulture),
            ["Method"] = context.Request.Method,
            ["Path"] = context.Request.Path,
            ["QueryString"] = context.Request.QueryString.Value,
            ["Endpoint"] = endpointDisplayName,
            ["TraceIdentifier"] = context.TraceIdentifier,
            ["CorrelationId"] = correlationId,
            ["AdditionalData"] = exception.AdditionalData == null ? null : JsonConvert.SerializeObject(exception.AdditionalData),
            ["StackTrace"] = exception.StackTrace
        };

        return JsonConvert.SerializeObject(details);
    }
}
