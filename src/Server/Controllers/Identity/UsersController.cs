using Application.Core.Identity.Users.Abstractions;
using Application.Core.Identity.Users.Models;

namespace Server.Controllers.Identity;

public class UsersController(IUserService userService) : BaseApiController
{
    private readonly IUserService _userService = userService;

    [HttpGet("me")]
    public async Task<Result<UserDetailsDto>> Me(CancellationToken cancellationToken)
    {
        return await _userService.Me(cancellationToken);
    }
}
