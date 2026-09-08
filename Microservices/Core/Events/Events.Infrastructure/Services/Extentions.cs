using Events.Application;
using Events.Application.Abstractions.Repositories;
using Events.Application.Abstractions.Services;
using Events.Application.Events.DTOs;
using Events.Domain.Entities;
using Events.Infrastructure.Services.Exceptions;
using Exceptions.Handling;
using Exceptions.Handling.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Events.Infrastructure.Services;

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
        services.AddScoped<IInboxRepository, InboxRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IReservedService, ReservedService>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddSingleton<ICacheOptions, CacheOption>();
        services.AddSingleton<IMessageBroker, MessageBroker>();
        services.AddHostedService<ConsumeReservedHostedService>();
        services.AddHostedService<OutboxHostedService>();
    }


    public static void AddExceptions(this IServiceCollection services)
    {
        services.AddGlobalExceptions();
        services.AddSingleton<IExceptionStatus, EventExceptionStatus>();
        services.AddSingleton<IExceptionStatus, NoAvailableSeatsExceptionStatus>();

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

    public static UserContext GetUser(this ClaimsPrincipal claimsIdentity)
    {
        var user = new UserContext();
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(Guid.TryParse(userId, out Guid result))
        {
            user.Id = result;
        }

        var role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;

        if(string.IsNullOrEmpty(role) == false)
            user.Role = role;

        var userLogin= claimsIdentity.FindFirst("preferred_username")?.Value;

        if(string.IsNullOrEmpty( userLogin) == false)
            user.Login = userLogin;

        return user;
    }
}

