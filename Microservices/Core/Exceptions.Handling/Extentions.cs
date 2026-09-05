using Exceptions.Handling.Abstractions;
using Exceptions.Handling.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Exceptions.Handling;

public static class Extentions
{
    /// <summary>
    /// Промежуточное ПО обработки исключений
    /// </summary>
    /// <param name="builder">Строитель</param>
    /// <returns>Строитель</returns>
    public static void UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }


    /// <summary>
    /// Подключение обработчиков исключений
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    public static void AddGlobalExceptions(this IServiceCollection services)
    {
        services.AddSingleton<IExceptionMediator, ExceptionMediatorService>();
        services.AddSingleton<IExceptionStatus, ValidateExceptionStatus>();
        
        services.AddSingleton<IExceptionStatus, CanceledExceptionStatus>();
    }
}
