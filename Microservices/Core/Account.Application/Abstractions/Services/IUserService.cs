using Account.Application.DTOs;
using Account.Domain.Entities;

namespace Account.Application.Abstractions.Services;
/// <summary>
/// Сервси управления пользователями
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Регистрация
    /// </summary>
    /// <param name="login">Логин</param>
    /// <param name="password">Пароль</param>
    /// <param name="role">Роль</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task Register(string login, string password, RoleType role, CancellationToken cancellationToken = default);
    /// <summary>
    /// Аутентификация
    /// </summary>
    /// <param name="login">Лоиг</param>
    /// <param name="password">Пароль</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Токен пользователя в случае положительной аутентификации</returns>
    Task<LoginResult?> Login(string login, string password, CancellationToken cancellationToken= default);
}
