namespace Account.Application.Abstractions.Services;
/// <summary>
/// Хэширование пароля
/// </summary>
public interface IPasswordHashing
{
    /// <summary>
    /// Хэшировать
    /// </summary>
    /// <param name="password">Пароль</param>
    /// <returns>Реззультат</returns>
    string Execure(string password);
}
