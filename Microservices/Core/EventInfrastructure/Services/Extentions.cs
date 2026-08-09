using EventApplication;
using EventApplication.Abstractions.Repositories;
using EventApplication.Abstractions.Services;
using EventDomain.Entities;
using EventInfrastructure.Abstractions;
using EventInfrastructure.Middlewares;
using EventInfrastructure.Services.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace EventInfrastructure.Services;

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

    public static void AddExceptions(this IServiceCollection services)
    {
        services.AddSingleton<IExceptionStatus, CanceledExceptionStatus>();
        services.AddScoped<IExceptionStatus, DefaultExceptionStatus>();
        services.AddScoped<IExceptionStatus, EventExceptionStatus>();
        services.AddScoped<IExceptionStatus, NoAvailableSeatsExceptionStatus>();
        services.AddScoped<IExceptionStatus, ValidateExceptionStatus>();
        services.AddScoped<IExceptionMediator, ExceptionMediatorService>();
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


    /// <summary>
    /// Фильтр списка
    /// </summary>
    /// <param name="events">Список событий</param>
    /// <param name="title">Наименование</param>
    /// <param name="from">Дата начала</param>
    /// <param name="to">Дата окончания</param>
    /// <returns>Список</returns>
    internal static IQueryable<Event> Filter(this IQueryable<Event> events, string? title, DateTime? from, DateTime? to)
    {

        var result = events;

        if (!string.IsNullOrEmpty(title))
            result = result.Where(s => EF.Functions.Like(s.Title, $"%{title}%"));

        if (from != null)
            result = result.Where(s => s.StartAt >= from);


        if (to != null)
        {
            if (from != null && from > to)
                throw new ValidationException("Дата окончания должна быть после даты начала");

            result = result.Where(s => s.EndAt <= to);
        }



        return result;
    }
}

