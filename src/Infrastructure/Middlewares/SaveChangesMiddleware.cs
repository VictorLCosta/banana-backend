using Application.Core.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Middlewares;

public class SaveChangesMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next.Invoke(context);

        var dbContext = context.RequestServices.GetRequiredService<IApplicationDbContext>();

        await dbContext.SaveChangesAsync(default);
    }
}
