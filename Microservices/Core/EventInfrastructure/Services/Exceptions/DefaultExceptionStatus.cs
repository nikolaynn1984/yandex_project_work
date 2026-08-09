using EventInfrastructure.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventInfrastructure.Services.Exceptions;

internal class DefaultExceptionStatus : IExceptionStatus
{
    public Type Type => typeof(Exception);

    public ProblemDetails Map(Exception ex)
    {
        return new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Ошибка  500  (Ошибки в сервере)",
            Detail = ""
        };
    }
}
