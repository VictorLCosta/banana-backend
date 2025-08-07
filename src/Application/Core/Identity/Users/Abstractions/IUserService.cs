using System.Security.Claims;

using Application.Core.Identity.Users.Features;
using Application.Core.Identity.Users.Models;

namespace Application.Core.Identity.Users.Abstractions;

public interface IUserService : IService
{
    Task<Result<UserDetailsDto>> Me(CancellationToken cancellationToken);

    Task<PagedList<UserDetailsDto>> SearchAsync(UserListFilter filter, CancellationToken cancellationToken);

    Task<Result<bool>> ExistsWithNameAsync(string name);
    Task<Result<bool>> ExistsWithEmailAsync(string email, string? exceptId = null);
    Task<Result<bool>> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null);

    Task<Result<List<UserDetailsDto>>> GetListAsync(CancellationToken cancellationToken);

    Task<Result<int>> GetCountAsync(CancellationToken cancellationToken);

    Task<Result<UserDetailsDto>> GetAsync(string userId, CancellationToken cancellationToken);

    Task<Result<List<UserRoleDto>>> GetRolesAsync(string userId, CancellationToken cancellationToken);
    Task<Result<string>> AssignRolesAsync(string userId, UserRolesRequest request, CancellationToken cancellationToken);

    Task<Result<List<string>>> GetPermissionsAsync(string userId, CancellationToken cancellationToken);
    Task<Result<bool>> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken = default);
    Task<Result> InvalidatePermissionCacheAsync(string userId, CancellationToken cancellationToken);

    Task<Result> ToggleStatusAsync(ToggleUserStatusRequest request, CancellationToken cancellationToken);

    Task<Result<string>> GetOrCreateFromPrincipalAsync(ClaimsPrincipal principal);
    Task<Result<string>> CreateAsync(CreateUserCommand command, string origin);
    Task<Result> UpdateAsync(UpdateUserRequest request, string userId);

    Task<Result<string>> ConfirmEmailAsync(string userId, string code, CancellationToken cancellationToken);
    Task<Result<string>> ConfirmPhoneNumberAsync(string userId, string code);

    Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);
    Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest request);
    Task<Result> ChangePasswordAsync(ChangePasswordRequest request, string userId);
}
