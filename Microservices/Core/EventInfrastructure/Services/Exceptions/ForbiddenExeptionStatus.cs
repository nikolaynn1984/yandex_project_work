using EventDomain.Exceptions;
using EventInfrastructure.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventInfrastructure.Services.Exceptions;

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
