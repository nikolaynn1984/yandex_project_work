namespace Account.Application.DTOs;
/// <summary>
/// Результат ответа аутентификации
/// </summary>
/// <param name="token">Токен</param>
public class LoginResult(string token)
{
    /// <summary>
    /// Токен
    /// </summary>
    public string Token { get; set; } = token;
}
