using Account.Application.DTOs;

namespace Account.Application.Abstractions.Services;
/// <summary>
/// Сервси управления пользователями
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Регистрация
    /// </summary>
    /// <param name="request">Параметры запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task Register(RegisterRequest request, CancellationToken cancellationToken = default);
    /// <summary>
    /// Аутентификация
    /// </summary>
    /// <param name="request">Параметры запроса</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Токен пользователя в случае положительной аутентификации</returns>
    Task<LoginResult?> Login(LoginRequest request, CancellationToken cancellationToken= default);
}
