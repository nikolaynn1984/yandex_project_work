using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

            httpContext.Response.StatusCode = error.Status ?? 0;
            httpContext.Response.ContentType = "application/json";

            error.Detail = ex.Message;
            error.Instance = httpContext.Request.Path;


            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(error));

        }

        private ProblemDetails MapStatusCode(Exception ex)
        {
            var result = new ProblemDetails();

            switch (ex)
            {
                case ValidationException ve:  
                    result.Status = StatusCodes.Status400BadRequest;
                    result.Title = "Ошибка 400 (Неверный запрос)";
                    break;
                case EventException eve:
                    result.Status = StatusCodes.Status404NotFound;
                    result.Title = "Ошибка 404 (Не найдено)";
                    break;
                default:
                    result.Status = StatusCodes.Status500InternalServerError;
                    result.Title = "Ошибка  500  (Ошибки в сервере)";
                    break;

            }

          return result;
        }
       
    }

}
