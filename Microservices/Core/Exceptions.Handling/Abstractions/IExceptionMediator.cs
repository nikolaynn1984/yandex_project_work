using Microsoft.AspNetCore.Mvc;

namespace Exceptions.Handling.Abstractions;

internal interface IExceptionMediator
{
    ProblemDetails Map<T>(T ex) where T : Exception;
}
