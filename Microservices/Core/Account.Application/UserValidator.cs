using Account.Application.Abstractions.Repositories;
using Account.Application.Abstractions.Services;
using Account.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Account.Application;

public class UserValidator : IUserValidator
{
    private readonly IUserRepository repository;

    public UserValidator(IUserRepository repository)
    {
        this.repository = repository;
    }

    public async Task IsUniqueLogin(string login, CancellationToken cancellationToken = default)
    {
        var user = await this.repository.GetByLogin(login, cancellationToken);
        if (user != null) 
            throw new ValidationException($"Логин {login} уже занят");
    }

    public void ThrowIfNull(User? user)
    {
        if (user == null)
            throw new ValidationException("Не верный логии и/или пароль");
    }
}
