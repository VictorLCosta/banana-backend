using Application.Core.Identity.Roles;
using Application.Core.Identity.Roles.Features;
using Application.Core.Identity.Users.Abstractions;

using Ardalis.Result;

using Infrastructure.Persistence;

namespace Infrastructure.Identity;

public class RoleService(
    RoleManager<ApplicationRole> roleManager, 
    UserManager<ApplicationUser> userManager, 
    ApplicationDbContext db,
    ICurrentUser currentUser) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _db = db;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Result<string>> CreateOrUpdateAsync(CreateOrUpdateRoleCommand command)
    {
        if (string.IsNullOrEmpty(command.Id))
        {
            var role = new ApplicationRole(command.Name, command.Description);
            var result = await _roleManager.CreateAsync(role);

            return $"Role {role.Name} created";
        }
        else 
        {
            var role = await _roleManager.FindByIdAsync(command.Id);

            if (role == null) return Result.NotFound("Role not found.");

            role.Name = command.Name;
            role.NormalizedName = command.Name.ToUpperInvariant();
            role.Description = command.Description;
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded) 
            {
                return Result.Error(new ErrorList(
                    result.Errors.Select(x => x.Description)
                ));
            }

            return $"Role {role.Name} updated";
        }
    }

    public async Task<Result<string>> DeleteAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null) return Result.NotFound("Role not found.");

        await _roleManager.DeleteAsync(role);

        return $"Role {role.Name} deleted";
    }

    public async Task<bool> ExistsAsync(string roleName, string? excludeId)=>
        await _roleManager.FindByNameAsync(roleName)
            is ApplicationRole existingRole
            && existingRole.Id != excludeId;

    public async Task<Result<RoleDto?>> GetByIdAsync(string id) =>
        await _db.Roles.SingleOrDefaultAsync(x => x.Id == id) is { } role
            ? role.Adapt<RoleDto>()
            : null;


    public async Task<Result<RoleDto?>> GetByIdWithPermissionsAsync(string roleId, CancellationToken cancellationToken)
    {
        var role = await GetByIdAsync(roleId);

        if (role.IsNotFound() || role.Value == null) return Result.NotFound("Role not found.");

        role.Value.Permissions = await _db.RoleClaims
            .Where(c => c.RoleId == roleId)
            .Select(c => c.ClaimValue!)
            .ToListAsync(cancellationToken);

        return role;
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken) =>
        await _roleManager.Roles.CountAsync(cancellationToken);

    public async Task<Result<List<RoleDto>>> GetListAsync(CancellationToken cancellationToken) =>
        Result.Success(
            (await _roleManager.Roles.ToListAsync(cancellationToken))
                .Adapt<List<RoleDto>>()
        );

    public async Task<Result<string>> UpdatePermissionsAsync(UpdatePermissionsCommand command, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(command.RoleId);

        if (role == null) return Result.NotFound("Role not found.");

        var currentClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var claim in currentClaims.Where(c => !command.Permissions.Any(p => p == c.Value)))
        {
            var removeResult = await _roleManager.RemoveClaimAsync(role, claim);
        }

        foreach (string permission in command.Permissions.Where(c => !currentClaims.Any(p => p.Value == c)))
        {
            if (!string.IsNullOrEmpty(permission))
            {
                _db.RoleClaims.Add(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = "permission",
                    ClaimValue = permission,
                    CreatedBy = _currentUser.GetUserId().ToString()
                });
                await _db.SaveChangesAsync(cancellationToken);
            }
        }

        return "Permissions updated";
    }
}
