using Microsoft.AspNetCore.Mvc;

namespace EventInfrastructure.Abstractions;

public interface IExceptionMediator
{
    ProblemDetails Map<T>(T ex) where T : Exception;
}
