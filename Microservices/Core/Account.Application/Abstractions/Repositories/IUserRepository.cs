using Account.Domain;

namespace Account.Application.Abstractions.Repositories;
/// <summary>
/// Репозиотрпий пользователя
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Регистрация
    /// </summary>
    /// <param name="login">Логин</param>
    /// <param name="passwordHas">Хэш пароль</param>
    /// <param name="role">Роль пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task Register(string login, string passwordHas, RoleType role, CancellationToken cancellationToken = default);
    /// <summary>
    /// Аутентификация пользователя
    /// </summary>
    /// <param name="login">Логин</param>
    /// <param name="passwordHas">Хэш пароль</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пользователь если найден</returns>
    Task<User> Login(string login, string passwordHas, CancellationToken cancellationToken = default);
}
