using Exceptions.Handling.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Exceptions.Handling.Services;

internal class CanceledExceptionStatus : IExceptionStatus
{
    public Type Type => typeof(OperationCanceledException);

    public ProblemDetails Map(Exception ex)
    {
        return new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Отмена операции",
            Detail = ex.Message
        };
    }
}
