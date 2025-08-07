namespace Application.Core.Identity.Roles.Features;

public record UpdatePermissionsCommand(string RoleId, List<string> Permissions);

public class UpdatePermissionsValidator : AbstractValidator<UpdatePermissionsCommand>
{
    public UpdatePermissionsValidator()
    {
        RuleFor(r => r.RoleId)
            .NotEmpty();
        RuleFor(r => r.Permissions)
            .NotNull();
    }
}
