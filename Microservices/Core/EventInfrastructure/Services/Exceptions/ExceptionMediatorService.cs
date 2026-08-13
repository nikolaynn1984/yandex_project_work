using EventInfrastructure.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventInfrastructure.Services.Exceptions;

public class ExceptionMediatorService : IExceptionMediator
{
    private readonly IEnumerable<IExceptionStatus> exceptions;


    public ExceptionMediatorService(IEnumerable<IExceptionStatus> exceptions)
    {
        this.exceptions = exceptions;
    }

    public ProblemDetails Map<T>(T ex) where T : Exception
    {
        IExceptionStatus? mapper = GetMapper<T>(ex);
        if (mapper == null)
            return GetDefault(ex);

        return mapper.Map(ex);

        
    }

    private IExceptionStatus? GetMapper<T>(T ex)
    {
        foreach (var exception in exceptions)
        {
            if (exception.Type == ex?.GetType())
            {
                return exception;
            }
        }

        return null;
    }


    private ProblemDetails GetDefault(Exception ex)
    {
        return new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Ошибка  500  (Ошибки в сервере)",
            Detail = ""
        };
    }
}
