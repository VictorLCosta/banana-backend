using Application.Core.Identity.Users.Abstractions;
using Application.Core.Identity.Users.Models;

using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers;

[AllowAnonymous]
public class UsersController(IUserService userService) : BaseApiController
{
    [HttpGet("me")]
    [AllowAnonymous]
    public async Task<Result<UserDetailsDto>> Me(CancellationToken cancellationToken)
    {
        return await userService.Me(cancellationToken);
    }
}
