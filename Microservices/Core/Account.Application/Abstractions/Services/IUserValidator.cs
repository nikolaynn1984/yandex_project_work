using Account.Domain.Entities;

namespace Account.Application.Abstractions.Services;
/// <summary>
/// Валидация пользователя
/// </summary>
public interface IUserValidator
{
    /// <summary>
    /// Проверить уникальность логина
    /// </summary>
    /// <param name="login">Логин</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task IsUniqueLogin(string login, CancellationToken cancellationToken = default);
    /// <summary>
    /// Если пользователь не пустой, выкинуть исключение валидации
    /// </summary>
    /// <param name="user">Пользователь</param>
    void ThrowIfNull(User? user);
}
