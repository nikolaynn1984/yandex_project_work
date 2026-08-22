using Bookings.Application;
using Bookings.Application.Abstractions.Repositories;
using Bookings.Application.Abstractions.Services;
using Bookings.Application.DTOs;
using Bookings.Infrastructure.Services;
using EventInfrastructure.Services.Exceptions;
using Exceptions.Handling;
using Exceptions.Handling.Abstractions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Security.AccessControl;
using System.Security.Claims;

namespace Bookings.Infrastructure;

public static class Extentions
{
    public static void AddBookingService(this IServiceCollection services)
    {
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IBookingValidator, BookingValidator>();
        services.AddSingleton<IMessageBroker, MessageBroker>();
        services.AddHostedService<BookingHostedService>();
        services.AddHostedService<ConsumeEventHostedService>();
    }

    public static void AddExceptions(this IServiceCollection services)
    {
        services.AddGlobalExceptions();

        services.AddSingleton<IExceptionStatus, BookingExceptionStatus>();
        services.AddSingleton<IExceptionStatus, ForbiddenExeptionStatus>();
        services.AddSingleton<IExceptionStatus, NoAvailableSeatsExceptionStatus>();
    }

    public static UserContext GetUser(this ClaimsPrincipal claimsIdentity)
    {
        var user = new UserContext();
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userId, out Guid result))
        {
            user.Id = result;
        }

        var role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(role) == false)
            user.Role = role;

        var userLogin = claimsIdentity.FindFirst("preferred_username")?.Value;

        if (string.IsNullOrEmpty(userLogin) == false)
            user.Login = userLogin;

        return user;
    }
}
