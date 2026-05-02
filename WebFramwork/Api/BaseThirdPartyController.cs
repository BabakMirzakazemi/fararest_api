using Microsoft.AspNetCore.Mvc;
using WebFramework.Filters;
using WebFramework.Filters.FluentValidationFilters;

namespace WebFramework.Api;

[ApiController]
[ApiResultFilter]
[FluentValidationFilter]
[Route("api/thirdParty/v{version:apiVersion}/[controller]")]// api/v1/[controller]
public class BaseThirdPartyController : ControllerBase
{

}
