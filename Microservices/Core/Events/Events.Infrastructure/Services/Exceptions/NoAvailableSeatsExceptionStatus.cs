using Events.Domain.Exceptions;
using Exceptions.Handling.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Events.Infrastructure.Services.Exceptions;

internal class NoAvailableSeatsExceptionStatus : IExceptionStatus
{
    public Type Type => typeof(NoAvailableSeatsException);

    public ProblemDetails Map(Exception ex)
    {
        return new ProblemDetails()
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Отмена операции",
            Detail = ex.Message
        };
    }
}
