using Account.Application;
using Account.Application.Abstractions.Repositories;
using Account.Application.Abstractions.Services;
using Account.Application.DTOs;
using EventApplication;
using EventApplication.Abstractions.Repositories;
using EventApplication.Abstractions.Services;
using EventApplication.Events.DTOs;
using EventDomain.Entities;
using EventInfrastructure.Abstractions;
using EventInfrastructure.DataAccess.Account;
using EventInfrastructure.Middlewares;
using EventInfrastructure.Services.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Principal;

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

    public static void AddAccount(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserValidator, UserValidator>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IPasswordHashing, PasswordHashing>();
    }

    public static void AddExceptions(this IServiceCollection services)
    {
        services.AddSingleton<IExceptionStatus, CanceledExceptionStatus>();
        
        services.AddSingleton<IExceptionStatus, EventExceptionStatus>();
        services.AddSingleton<IExceptionStatus, NoAvailableSeatsExceptionStatus>();
        services.AddSingleton<IExceptionStatus, ValidateExceptionStatus>();
        services.AddSingleton<IExceptionStatus, ForbiddenExeptionStatus>();
        services.AddSingleton<IExceptionMediator, ExceptionMediatorService>();
    }

    public static void AddBookingService(this IServiceCollection services)
    {
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IBookingValidator, BookingValidator>();
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

