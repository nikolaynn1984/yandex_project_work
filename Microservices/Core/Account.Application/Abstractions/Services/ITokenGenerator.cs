namespace Account.Application.Abstractions.Services;
/// <summary>
/// Сервси генератции токена
/// </summary>
public interface ITokenGenerator
{
    /// <summary>
    /// Генерировать токен
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="login">Логин</param>
    /// <param name="role">Роль</param>
    /// <returns>Токен</returns>
    string Generate(Guid userId, string login, string role);
}
