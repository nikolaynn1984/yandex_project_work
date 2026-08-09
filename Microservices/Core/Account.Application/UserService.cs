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
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="repository">Репоззиторий</param>
    /// <param name="tokenGenerator">Генератор токена</param>
    /// <param name="hashing">Хэширование пароля</param>
    public UserService(IUserRepository repository, ITokenGenerator tokenGenerator, IPasswordHashing hashing)
    {
        this.repository = repository;
        this.tokenGenerator = tokenGenerator;
        this.hashing = hashing;
    }

    public async Task Register(string login, string password, RoleType role, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        string hash = this.hashing.Execure(password);

        await this.repository.Register(login, hash, role, cancellationToken);
    }

    public async Task<LoginResult?> Login(string login, string password, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return null;

        string hash = this.hashing.Execure(password);

        var user = await this.repository.Login(login, hash, cancellationToken);

        string token = string.Empty;

        if(user != null)
        {
            token = this.tokenGenerator.Generate(user.Id, user.Login, user.Role.ToString());
        }


        return new LoginResult(token);
    }
}
