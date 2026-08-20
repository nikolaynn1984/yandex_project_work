using Microsoft.AspNetCore.Mvc;

namespace Exceptions.Handling.Abstractions;

public interface IExceptionMediator
{
    ProblemDetails Map<T>(T ex) where T : Exception;
}
