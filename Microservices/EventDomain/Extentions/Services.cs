using EventDomain.Interfaces;
using EventDomain.Repository;
using EventDomain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace EventDomain.Extentions;

/// <summary>
/// Сервисы
/// </summary>
public static class Services
{
    /// <summary>
    /// Добавление сервсива обработки событий
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    public static void AddEventService(this IServiceCollection services)
    {
        services.AddScoped<IEventService, EventService>();
    }

    public static void AddBookingService(this IServiceCollection services)
    {
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddSingleton<IBookingQueueService, BookingQueueService>();
        services.AddHostedService<BookingHostedService>();
    }

    /// <summary>
    /// Промежуточное ПО обработки исключений
    /// </summary>
    /// <param name="builder">Строитель</param>
    /// <returns>Строитель</returns>
    public static void UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
