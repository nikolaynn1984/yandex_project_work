using Account.Application;
using Account.Application.Abstractions.Repositories;
using Account.Application.Abstractions.Services;
using EventInfrastructure.DataAccess.Account;
using EventInfrastructure.Services;
using Exceptions.Handling;
using Exceptions.Handling.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Infrastructure.Services;

public static class Extentions
{
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
        services.AddGlobalExceptions();
    }
}
