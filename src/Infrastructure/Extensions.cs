using Infrastructure.Auth;
using Infrastructure.Cors;
using Infrastructure.Logging;
using Infrastructure.Middlewares;
using Infrastructure.Persistence;
using Infrastructure.RateLimit;
using Infrastructure.SecurityHeaders;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Extensions
{
    public static WebApplicationBuilder AddInfratructure(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        builder.ConfigureSerilog(configuration);

        builder.Services.ConfigureRateLimit(configuration);

        builder.Services.ConfigureSecurityHeaders(configuration);

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        builder.AddPersistence(configuration);

        builder.Services.AddMapster();

        builder.Services.AddRouting(opt => opt.LowercaseUrls = true);

        builder.Services.AddMiddlewares();

        builder.Services.AddAuth(configuration);

        builder.Services.AddCorsPolicy(configuration);

        return builder;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IConfiguration configuration) =>
        app
            .UseStaticFiles()
            .UseSecurityHeaders()
            .UseRouting()
            .UseCorsPolicy()
            .UseRateLimit()
            .UseMiddlewares()
            .UseAuthentication()
            .UseAuthorization();
    
    public static async Task InitializeDatabasesAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        // Create a new scope to retrieve scoped services
        using var scope = services.CreateScope();

        await scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>()
            .InitialiseAsync(cancellationToken);
    }
}
