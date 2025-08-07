using Application.Common.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Serilog.Context;

namespace Infrastructure.Middlewares;

public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger) : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var problemDetails = new ProblemDetails
            {
                Instance = context.Request.Path
            };

            if (exception is CustomAppException e)
            {
                context.Response.StatusCode = (int)e.StatusCode;
                problemDetails.Detail = e.Message;

                if (e.ErrorMessages != null && e.ErrorMessages.Count != 0)
                {
                    problemDetails.Extensions.Add("errors", e.ErrorMessages);
                }
            }
            else
            {
                problemDetails.Detail = exception.Message;
            }

            string errorId = Guid.NewGuid().ToString();
            LogContext.PushProperty("ErrorId", errorId);
            LogContext.PushProperty("StackTrace", exception.StackTrace);
            LogContext.PushProperty("StackTrace", exception.StackTrace);
            _logger.LogError("{ProblemDetail}", problemDetails.Detail);
            await context.Response.WriteAsJsonAsync(problemDetails).ConfigureAwait(false);
        }
    }
}
