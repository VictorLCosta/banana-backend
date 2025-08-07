using System.Net;

namespace Server;

public static class Extensions
{
    public static WebApplicationBuilder AddServer(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddControllers(mvcOptions => mvcOptions
            .AddResultConvention(resultStatusMap => resultStatusMap
                .AddDefaultMap()
                .For(ResultStatus.Ok, HttpStatusCode.OK, resultStatusOptions => resultStatusOptions
                    .For("DELETE", HttpStatusCode.NoContent))
                .For(ResultStatus.Created, HttpStatusCode.Created, resultStatusOptions => resultStatusOptions
                    .For("POST", HttpStatusCode.Created))
            ));

        return builder;
    }
}
