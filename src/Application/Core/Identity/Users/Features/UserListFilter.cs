namespace Application.Core.Identity.Users.Features;

public class UserListFilter : PaginationFilter
{
    public bool? IsActive { get; set; }
}
