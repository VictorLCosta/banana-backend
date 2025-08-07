using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace Infrastructure.Logging;

public static class Extensions
{
    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Host.UseSerilog((context, logger) => 
        {
            var connectionString = configuration.GetConnectionString("BaseLog");

            logger.ReadFrom.Configuration(context.Configuration);

            ConfigureEnrichers(logger, "Banana");
            ConfigureConsoleLogging(logger, false);
            ConfigureSqlite(builder, logger, connectionString!);
            OverideMinimumLogLevel(logger);
            SetMinimumLogLevel(logger, "Information");
        });

        return builder;
    }

    private static void ConfigureEnrichers(LoggerConfiguration serilogConfig, string appName)
    {
        serilogConfig
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", appName)
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.FromLogContext();
    }

    private static void ConfigureConsoleLogging(LoggerConfiguration serilogConfig, bool structuredConsoleLogging)
    {
        if (structuredConsoleLogging)
        {
            serilogConfig.WriteTo.Async(wt => wt.Console(new CompactJsonFormatter()));
        }
        else
        {
            serilogConfig.WriteTo.Async(wt => wt.Console());
        }
    }

    private static void ConfigureSqlite(WebApplicationBuilder builder, LoggerConfiguration serilogConfig, string connectionString)
    {
        var sqlPath = builder.Environment.ContentRootPath;

        serilogConfig.WriteTo.Async(writeTo =>
            writeTo
                .SQLite(
                    sqliteDbPath: sqlPath + connectionString,
                    tableName: "ApplicationLogs"
                )
                .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName!)
        );
    }

    private static void OverideMinimumLogLevel(LoggerConfiguration serilogConfig)
    {
        serilogConfig
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Hangfire", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error);
    }

    private static void SetMinimumLogLevel(LoggerConfiguration serilogConfig, string minLogLevel)
    {
        switch (minLogLevel.ToLower())
        {
            case "debug":
                serilogConfig.MinimumLevel.Debug();
                break;
            case "information":
                serilogConfig.MinimumLevel.Information();
                break;
            case "warning":
                serilogConfig.MinimumLevel.Warning();
                break;
            default:
                serilogConfig.MinimumLevel.Information();
                break;
        }
    }
}
