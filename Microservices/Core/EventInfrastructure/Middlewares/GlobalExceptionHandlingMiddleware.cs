using EventDomain.Exceptions;
using EventInfrastructure.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.Arm;
using System.Text.Json;

namespace EventInfrastructure.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> logger;
    private readonly IExceptionMediator exception;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger, IExceptionMediator exception)
    {
        this.next = next;
        this.logger = logger;
        this.exception = exception; 
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            Guid correlationToken = Guid.NewGuid();
            string? requestToken = httpContext.Request.Headers["Correlation-Token"];
            if (!(requestToken != null && Guid.TryParse(httpContext.Request.Headers["Correlation-Token"], out correlationToken)))
            {
                correlationToken = Guid.NewGuid();
                httpContext.Request.Headers.TryAdd("Correlation-Token", correlationToken.ToString());
            }

            await this.next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleException(httpContext, ex);
        }
    }


    private async Task HandleException(HttpContext httpContext, Exception ex)
    {
        this.logger.LogError(ex, "Необработанное исключение. Method={Method}, Path={Path}, Correlation-Token={RequestId}",
        httpContext.Request.Method,
        httpContext.Request.Path,
        httpContext.Request.Headers["Correlation-Token"]);

        if (httpContext.Response.HasStarted)
        {
            return;
        }

        var error = this.exception.Map(ex);

        httpContext.Response.StatusCode = error.Status ?? 0;
        httpContext.Response.ContentType = "application/problem+json";

        //error.Detail = ex.Message;
        error.Instance = httpContext.Request.Path;


        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(error));

    }
}
