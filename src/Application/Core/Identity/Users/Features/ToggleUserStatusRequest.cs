namespace Application.Core.Identity.Users.Features;

public record ToggleUserStatusRequest(bool ActivateUser, string? UserId);
