using Application.Core.Identity.Roles.Features;

namespace Application.Core.Identity.Roles;

public interface IRoleService : IService
{
    Task<Result<List<RoleDto>>> GetListAsync(CancellationToken cancellationToken);

    Task<int> GetCountAsync(CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string roleName, string? excludeId);

    Task<Result<RoleDto?>> GetByIdAsync(string id);

    Task<Result<RoleDto?>> GetByIdWithPermissionsAsync(string roleId, CancellationToken cancellationToken);

    Task<Result<string>> CreateOrUpdateAsync(CreateOrUpdateRoleCommand command);

    Task<Result<string>> UpdatePermissionsAsync(UpdatePermissionsCommand command, CancellationToken cancellationToken);

    Task<Result<string>> DeleteAsync(string id);
}
