using Microsoft.AspNetCore.Mvc;
using WebFramework.Filters;

namespace WebFramework.Api;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiResultFilter]
public abstract class BaseCommonController : ControllerBase
{
}
