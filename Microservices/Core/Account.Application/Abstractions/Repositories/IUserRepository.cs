using Account.Domain.Entities;

namespace Account.Application.Abstractions.Repositories;
/// <summary>
/// Репозиотрпий пользователя
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Регистрация
    /// </summary>
    /// <param name="user">Новый пользователь</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task Register(User user, CancellationToken cancellationToken = default);
    /// <summary>
    /// Аутентификация пользователя
    /// </summary>
    /// <param name="login">Логин</param>
    /// <param name="passwordHas">Хэш пароль</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пользователь если найден</returns>
    Task<User?> Login(string login, string passwordHas, CancellationToken cancellationToken = default);
    /// <summary>
    /// Получить пользователя по логину
    /// </summary>
    /// <param name="login">Лоин</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пользователь если найден</returns>
    Task<User?> GetByLogin(string login, CancellationToken cancellationToken = default);
}
