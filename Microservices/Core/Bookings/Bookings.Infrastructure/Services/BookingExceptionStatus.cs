using Bookings.Domain.Exceptions;
using Exceptions.Handling.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookings.Infrastructure.Services;

internal class BookingExceptionStatus : IExceptionStatus
{
    public Type Type => typeof(BookingException);

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
