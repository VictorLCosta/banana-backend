using Application.Core.Identity.Users.Abstractions;

using Infrastructure.Auth.Jwt;
using Infrastructure.Auth.Permissions;
using Infrastructure.Identity;
using Infrastructure.Middlewares;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Auth;

internal static class Extensions
{
    internal static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddCurrentUser()
            .AddPermissions()
            .AddIdentity()
            .AddJwtAuth();

    private static IServiceCollection AddCurrentUser(this IServiceCollection services) =>
        services
            .AddScoped<CurrentUserMiddleware>()
            .AddScoped<ICurrentUser, CurrentUser>()
            .AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());

    private static IServiceCollection AddPermissions(this IServiceCollection services) =>
        services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
}
