using Common.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
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
        //ApiResultStatusCode apiStatusCode = ApiResultStatusCode.ServerError;

        try
        {
            await _next(context);
        }
        catch (AppException exception)
        {
            _logger.LogError(exception, exception.Message);
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
    }
}
