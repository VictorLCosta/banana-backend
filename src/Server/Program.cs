using Application;

using Infrastructure;
using Infrastructure.Logging;

using Scalar.AspNetCore;

using Serilog;

using Server;


StaticLogger.EnsureInitialized();
Log.Information("server booting up..");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder
        .AddApplication()
        .AddInfratructure(builder.Configuration)
        .AddServer();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference("api-docs");
    }

    app.UseInfrastructure(builder.Configuration);
    await app.Services.InitializeDatabasesAsync();

    app.UseHttpsRedirection();

    await app.RunAsync();
}
catch (Exception ex)
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex.Message, "unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("server shutting down..");
    await Log.CloseAndFlushAsync();
}
