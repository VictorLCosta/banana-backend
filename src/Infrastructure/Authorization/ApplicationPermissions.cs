using System.Collections.ObjectModel;

namespace Infrastructure.Authorization;

public static class AppAction
{
    public const string View = nameof(View);
    public const string Search = nameof(Search);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
    public const string Export = nameof(Export);
    public const string Generate = nameof(Generate);
    public const string Clean = nameof(Clean);
    public const string UpgradeSubscription = nameof(UpgradeSubscription);
}

public static class AppResource
{
    public const string Users = nameof(Users);
    public const string UserRoles = nameof(UserRoles);
    public const string Roles = nameof(Roles);
    public const string RoleClaims = nameof(RoleClaims);
}

public class ApplicationPermissions
{
    private static readonly ApplicationPermission[] _allPermissions = 
    [
        new("View Myself", AppAction.View, AppResource.Users),
        new("View Users", AppAction.View, AppResource.Users),
        new("Search Users", AppAction.Search, AppResource.Users),
        new("Create User", AppAction.Create, AppResource.Users),
        new("Update User", AppAction.Update, AppResource.Users),
        new("Delete User", AppAction.Delete, AppResource.Users),
        new("View User Roles", AppAction.View, AppResource.UserRoles),
        new("Search User Roles", AppAction.Search, AppResource.UserRoles),
        new("Create User Role", AppAction.Create, AppResource.UserRoles),
        new("Update User Role", AppAction.Update, AppResource.UserRoles),
        new("Delete User Role", AppAction.Delete, AppResource.UserRoles),
        new("View Roles", AppAction.View, AppResource.Roles),
        new("Search Roles", AppAction.Search, AppResource.Roles),
        new("Create Role", AppAction.Create, AppResource.Roles),
        new("Update Role", AppAction.Update, AppResource.Roles),
        new("Delete Role", AppAction.Delete, AppResource.Roles),
        new("View Role Claims", AppAction.View, AppResource.RoleClaims),
        new("Search Role Claims", AppAction.Search, AppResource.RoleClaims),
        new("Create Role Claim", AppAction.Create, AppResource.RoleClaims),
        new("Update Role Claim", AppAction.Update, AppResource.RoleClaims),
    ];

    public static IReadOnlyList<ApplicationPermission> All { get; } = new ReadOnlyCollection<ApplicationPermission>(_allPermissions);
    public static IReadOnlyList<ApplicationPermission> Root { get; } = new ReadOnlyCollection<ApplicationPermission>(_allPermissions.Where(p => p.IsRoot).ToArray());
    public static IReadOnlyList<ApplicationPermission> Admin { get; } = new ReadOnlyCollection<ApplicationPermission>(_allPermissions.Where(p => !p.IsRoot).ToArray());
    public static IReadOnlyList<ApplicationPermission> Basic { get; } = new ReadOnlyCollection<ApplicationPermission>(_allPermissions.Where(p => p.IsBasic).ToArray());
}

public record ApplicationPermission(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false)
{
    public string Name => NameFor(Action, Resource);
    public static string NameFor(string action, string resource) => $"Permissions.{resource}.{action}";
}
