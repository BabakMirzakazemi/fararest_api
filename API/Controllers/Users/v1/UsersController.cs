using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts.Authentications;
using Services.DTOs.Common;
using Services.DTOs.Users;
using WebFramework.Api;
using WebFramework.Filters;

namespace API.Controllers.Users.v1
{
    [ApiVersion("1")]
    //[ApiCustomAuthorize(false, RoleHelper.User)]
    public class UsersController (IUserService userService) : BaseUserApiController
    {
        [HttpPost("[action]")]
        [ApiPermissionAuthorize("accounts.view")]
        public async Task<PagingDTO<UserListDTO>> AllPaginatedAsync(FilterUserRequest dto, CancellationToken cancellationToken)
                     => await userService.All(dto, cancellationToken);

        [HttpPost("[action]")]
        [ApiPermissionAuthorize("accounts.view")]
        public async Task<CursorPagingDTO<UserListDTO>> AllCursorAsync(FilterUserCursorRequest dto, CancellationToken cancellationToken)
                     => await userService.AllByCursor(dto, cancellationToken);
    }
}
