using System.Reflection;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class Extensions
{
    public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        builder
            .Services
            .AddMediatR(assembly)
            .AddValidatorsFromAssembly(assembly, includeInternalTypes: true)
            .AddServices();

        return builder;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var infrastructureAssembly = Assembly.Load("Infrastructure");

        var serviceTypes = assembly
            .GetTypes()
            .Where(x => typeof(IService).IsAssignableFrom(x) && x.IsInterface && x != typeof(IService));

        foreach (var serviceType in serviceTypes)
        {
            var implementationType = infrastructureAssembly.GetTypes()
                .FirstOrDefault(t => serviceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            if (implementationType != null)
            {
                services.AddTransient(serviceType, implementationType);
            }
        }

        return services;
    }
}
