using Account.Application.Abstractions.Repositories;
using Account.Application.Abstractions.Services;
using Account.Application.DTOs;
using Account.Domain.Entities;

namespace Account.Application;
/// <summary>
/// Сервис управления пользователя
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository repository;
    private readonly ITokenGenerator tokenGenerator;
    private readonly IPasswordHashing hashing;
    private readonly IUserValidator userValidator;
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="repository">Репоззиторий</param>
    /// <param name="tokenGenerator">Генератор токена</param>
    /// <param name="hashing">Хэширование пароля</param>
    public UserService(IUserRepository repository, ITokenGenerator tokenGenerator, IPasswordHashing hashing, IUserValidator userValidator)
    {
        this.repository = repository;
        this.tokenGenerator = tokenGenerator;
        this.hashing = hashing;
        this.userValidator = userValidator;
    }

    public async Task Register(string login, string password, RoleType role, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        await this.userValidator.IsUniqueLogin(login, cancellationToken);

        var id = Guid.NewGuid();
        string hash = this.hashing.Execure(password);

        await this.repository.Register(new User() { Id = id, Login = login, PasswordHash = hash,Role = role}, cancellationToken);
    }

    public async Task<LoginResult?> Login(string login, string password, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return null;

        string hash = this.hashing.Execure(password);

        var user = await this.repository.Login(login, hash, cancellationToken);

        this.userValidator.ThrowIfNull(user);

#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
        string token = this.tokenGenerator.Generate(user.Id, user.Login, user.Role.ToString());
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.


        return new LoginResult(token);
    }
}
