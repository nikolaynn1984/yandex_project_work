using Account.Application.Abstractions.Repositories;
using Account.Application.Abstractions.Services;
using Account.Application.DTOs;
using Account.Domain;

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

    public async Task Register(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return;
        var id = Guid.NewGuid();

        await this.userValidator.IsUniqueLogin(request.Login, cancellationToken);


        string hash = this.hashing.Execure(request.Password);

        await this.repository.Register(new User() { Id = id, Login = request.Login, PasswordHash = hash,Role = request.Role }, cancellationToken);
    }

    public async Task<LoginResult?> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return null;

        string hash = this.hashing.Execure(request.Password);

        var user = await this.repository.Login(request.Login, hash, cancellationToken);

        this.userValidator.ThrowIfNull(user);

#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
        string token = this.tokenGenerator.Generate(user.Id, user.Login, user.Role.ToString());
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.


        return new LoginResult(token);
    }
}
