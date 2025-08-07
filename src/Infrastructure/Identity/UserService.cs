using Application.Core.Identity.Users.Abstractions;
using Application.Core.Identity.Users.Features;
using Application.Core.Identity.Users.Models;
using Application.Core.Models;

using Infrastructure.Persistence;

namespace Infrastructure.Identity;

public partial class UserService(
    RoleManager<ApplicationRole> roleManager, 
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager, 
    ApplicationDbContext db,
    ICurrentUser currentUser
) : IUserService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager= signInManager;
    private readonly ApplicationDbContext _db = db;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Result<UserDetailsDto>> Me(CancellationToken cancellationToken)
    {
        if (_currentUser.IsAuthenticated())
        {
            var currentUser = await _userManager.FindByIdAsync(_currentUser.GetUserId().ToString());

            if (currentUser == null) 
                return Result.NotFound("User not found.");

            return currentUser.Adapt<UserDetailsDto>();
        }

        return Result.Forbidden();
    }

    public async Task<Result<bool>> ExistsWithNameAsync(string name) => 
        Result.Success(await _userManager.FindByNameAsync(name) is not null);

    public async Task<Result<bool>> ExistsWithEmailAsync(string email, string? exceptId = null) =>
        Result.Success(await _userManager.FindByEmailAsync(email.Normalize()) is ApplicationUser user && user.Id != exceptId);

    public Task<Result<bool>> ExistsWithPhoneNumberAsync(string phoneNumber, string? exceptId = null)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<UserDetailsDto>> GetAsync(string userId, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (users == null)
            return Result<UserDetailsDto>.NotFound("Usuário não encontrado.");

        return Result.Success(users.Adapt<UserDetailsDto>());
    }

    public async Task<Result<int>> GetCountAsync(CancellationToken cancellationToken) =>
        Result.Success(await _userManager.Users.AsNoTracking().CountAsync(cancellationToken));

    public async Task<Result<List<UserDetailsDto>>> GetListAsync(CancellationToken cancellationToken) =>
        Result.Success(
            (await _userManager.Users.AsNoTracking().ToListAsync(cancellationToken)).Adapt<List<UserDetailsDto>>()
        );

    public async Task<PagedList<UserDetailsDto>> SearchAsync(UserListFilter filter, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users
            .PaginatedListAsync<ApplicationUser, UserDetailsDto>(x => x.IsActive == filter.IsActive, filter.PageNumber, filter.PageSize, cancellationToken);
            
        int count = await _userManager.Users
            .CountAsync(cancellationToken);

        return users;
    }

    public async Task<Result> ToggleStatusAsync(ToggleUserStatusRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            return Result.NotFound("User not found.");

        bool isAdmin = await _userManager.IsInRoleAsync(user, "");
        if (isAdmin)
        {
            return Result.Conflict("Administrators Profile's Status cannot be toggled");
        }

        user.IsActive = request.ActivateUser;

        await _userManager.UpdateAsync(user);

        return Result.NoContent();
    }

}
