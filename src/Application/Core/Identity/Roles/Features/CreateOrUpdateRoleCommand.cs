namespace Application.Core.Identity.Roles.Features;

public record CreateOrUpdateRoleCommand(string Id, string Name, string? Description);

public class CreateOrUpdateRoleCommandValidator : AbstractValidator<CreateOrUpdateRoleCommand>
{
    public CreateOrUpdateRoleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Role name is required.");
    }
}