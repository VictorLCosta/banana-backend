using Application.Core.Identity.Users.Features;
using Application.Core.Identity.Users.Models;

namespace Infrastructure.Identity;

public partial class UserService
{
    public async Task<Result<List<UserRoleDto>>> GetRolesAsync(string userId, CancellationToken cancellationToken)
    {
        var userRoles = new List<UserRoleDto>();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<List<UserRoleDto>>.NotFound("User Not Found.");
        }

        var roles = await _roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken);
        if (roles == null || roles.Count == 0)
        {
            return Result<List<UserRoleDto>>.NotFound("Roles Not Found.");
        }

        foreach (var role in roles)
        {
            userRoles.Add(new UserRoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Description = role.Description,
                Enabled = await _userManager.IsInRoleAsync(user, role.Name!)
            });
        }

        return Result<List<UserRoleDto>>.Success(userRoles);
    }

    public async Task<Result<string>> AssignRolesAsync(string userId, UserRolesRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            return Result<string>.NotFound("User Not Found.");
        }

        // Check if the user is an admin for which the admin role is getting disabled
        if (await _userManager.IsInRoleAsync(user, "") &&
            request.UserRoles.Any(a => !a.Enabled && a.RoleName == ""))
        {
            int adminCount = (await _userManager.GetUsersInRoleAsync("")).Count;

            if (adminCount <= 2)
            {
                return Result<string>.Conflict("Tenant should have at least 2 Admins.");
            }
        }

        foreach (var userRole in request.UserRoles)
        {
            if (await _roleManager.FindByNameAsync(userRole.RoleName!) != null)
            {
                if (userRole.Enabled)
                {
                    if (!await _userManager.IsInRoleAsync(user, userRole.RoleName!))
                    {
                        await _userManager.AddToRoleAsync(user, userRole.RoleName!);
                    }
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, userRole.RoleName!);
                }
            }
        }

        return Result<string>.Success("User Roles Updated Successfully.");
    }
}
