using Events.Domain.Exceptions;
using Exceptions.Handling.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Events.Infrastructure.Services.Exceptions;

internal class EventExceptionStatus : IExceptionStatus
{
    public Type Type => typeof(EventException);

    public ProblemDetails Map(Exception ex)
    {
        return new ProblemDetails()
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Ошибка 404 (Не найдено)",
            Detail = ex.Message
        };
    }
}
