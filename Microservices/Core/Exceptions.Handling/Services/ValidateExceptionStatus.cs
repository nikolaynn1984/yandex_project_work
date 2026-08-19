using Exceptions.Handling.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Exceptions.Handling.Services;

internal class ValidateExceptionStatus : IExceptionStatus
{
    public Type Type { get => typeof(ValidationException);}

    public ProblemDetails Map(Exception ex)
    {
        return new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Ошибка 400 (Неверный запрос)",
            Detail = ex.Message
        };
    }
}
