using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using WebFramework.Api;

namespace WebFramework.Filters;

public class ApiResultFilterAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is OkObjectResult okObjectResult)
        {
            var newApiResult = new ApiResult<object>(true, HttpStatusCode.OK, okObjectResult.Value);
            context.Result = new JsonResult(newApiResult) { StatusCode = okObjectResult.StatusCode };
        }
        else if (context.Result is OkResult okResult)
        {
            var newApiResult = new ApiResult(true, HttpStatusCode.OK);
            context.Result = new JsonResult(newApiResult) { StatusCode = okResult.StatusCode };
        }
        else if (context.Result is EmptyResult emptyResult)
        {
            var newApiResult = new ApiResult(true, HttpStatusCode.OK);
            context.Result = new JsonResult(newApiResult) { StatusCode = (int)HttpStatusCode.OK };
        }
        else if (context.Result is ObjectResult badRequestObjectResult && badRequestObjectResult.StatusCode == 400)
        {
            string? message = null;
            switch (badRequestObjectResult.Value)
            {
                case ValidationProblemDetails validationProblemDetails:
                    var errorMessages = validationProblemDetails.Errors.SelectMany(p => p.Value).Distinct();
                    message = string.Join(" | ", errorMessages);
                    break;
                case SerializableError errors:
                    var errorMessages2 = errors
                        .SelectMany(p => p.Value as string[] ?? Array.Empty<string>())
                        .Distinct();
                    message = string.Join(" | ", errorMessages2);
                    break;
                case var value when value != null && !(value is ProblemDetails):
                    message = value.ToString();
                    break;
            }

            var newApiResult = new ApiResult(false, HttpStatusCode.BadRequest, message);
            context.Result = new JsonResult(newApiResult) { StatusCode = badRequestObjectResult.StatusCode };
        }
        else if (context.Result is ObjectResult notFoundObjectResult && notFoundObjectResult.StatusCode == 404)
        {
            string? message = null;
            if (notFoundObjectResult.Value != null && !(notFoundObjectResult.Value is ProblemDetails))
                message = notFoundObjectResult.Value.ToString();

            var newApiResult = new ApiResult(false, HttpStatusCode.NotFound, message);
            context.Result = new JsonResult(newApiResult) { StatusCode = notFoundObjectResult.StatusCode };
        }
        else if (context.Result is ContentResult contentResult)
        {
            var newApiResult = new ApiResult(true, HttpStatusCode.OK, contentResult.Content);
            context.Result = new JsonResult(newApiResult) { StatusCode = contentResult.StatusCode };
        }
        else if (context.Result is ObjectResult objectResult && objectResult.StatusCode == null
            && !(objectResult.Value is ApiResult))
        {
            var newApiResult = new ApiResult<object>(true, HttpStatusCode.OK, objectResult.Value);
            context.Result = new JsonResult(newApiResult) { StatusCode = objectResult.StatusCode };
        }

        base.OnResultExecuting(context);
    }
}
