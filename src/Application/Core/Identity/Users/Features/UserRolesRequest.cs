using Application.Core.Identity.Users.Models;

namespace Application.Core.Identity.Users.Features;

public class UserRolesRequest
{
    public List<UserRoleDto> UserRoles { get; set; } = new();
}
