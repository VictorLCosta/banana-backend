using System.Security.Claims;

namespace Application.Core.Identity.Users.Abstractions;

public interface ICurrentUserInitializer
{
    void SetCurrentUser(ClaimsPrincipal user);

    void SetCurrentUserId(Guid userId);
}
