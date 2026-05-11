using EventDomain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace EventDomain.Extentions
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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

                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleException(httpContext, ex);
            }
        }


        private async Task HandleException(HttpContext httpContext, Exception ex)
        {
            _logger.LogError( ex, "Необработанное исключение. Method={Method}, Path={Path}, Correlation-Token={RequestId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            httpContext.Request.Headers["Correlation-Token"]);

            if (httpContext.Response.HasStarted)
            {
                return;
            }

            var error = MapStatusCode(ex);

            httpContext.Response.StatusCode = error.StatusCode;
            httpContext.Response.ContentType = "application/json";

            error.Message = ex.Message;


            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(error));

        }

        private ErrorResponce MapStatusCode(Exception ex)
        {
            var result = new ErrorResponce();

            switch (ex)
            {
                case ValidationException ve:  
                    result.StatusCode = StatusCodes.Status400BadRequest;
                    result.ErrorType = StatusCodes.Status400BadRequest.ToString();
                    break;
                case EventException eve:
                    result.StatusCode = StatusCodes.Status404NotFound;
                    result.ErrorType = StatusCodes.Status404NotFound.ToString();
                    break;
                default:
                    result.StatusCode = StatusCodes.Status500InternalServerError;
                    result.ErrorType = StatusCodes.Status500InternalServerError.ToString();
                    break;

            }

          return result;
        }
       
    }

}
