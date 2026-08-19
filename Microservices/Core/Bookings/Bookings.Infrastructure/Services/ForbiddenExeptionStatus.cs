using Bookings.Domain.Exceptions;
using Exceptions.Handling.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookings.Infrastructure.Services;

internal class ForbiddenExeptionStatus : IExceptionStatus
{
    public Type Type => typeof(ForbiddenExeption);

    public ProblemDetails Map(Exception ex)
    {
        return new ProblemDetails()
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Не достаточно прав"
        };
    }
}
