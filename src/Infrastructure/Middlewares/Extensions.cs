using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Middlewares;

internal static class Extensions
{
    internal static IServiceCollection AddMiddlewares(this IServiceCollection services) =>
        services
            .AddScoped<SaveChangesMiddleware>()
            .AddScoped<ExceptionMiddleware>()
            .AddScoped<CurrentUserMiddleware>()
            .AddScoped<RequestLoggingMiddleware>()
            .AddScoped<ResponseLoggingMiddleware>();

    internal static IApplicationBuilder UseMiddlewares(this IApplicationBuilder app) =>
        app
            .UseMiddleware<SaveChangesMiddleware>()
            .UseMiddleware<ExceptionMiddleware>()
            .UseMiddleware<CurrentUserMiddleware>()
            .UseMiddleware<RequestLoggingMiddleware>()
            .UseMiddleware<ResponseLoggingMiddleware>();
}
