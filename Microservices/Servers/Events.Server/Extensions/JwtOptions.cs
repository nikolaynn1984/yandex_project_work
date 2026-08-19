namespace Events.Server.Extensions;

/// <summary>
/// Параметры Jwt
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Издатель
    /// </summary>
    public string Issuer { get; set; } = "Event";
    /// <summary>
    /// Кому выдан
    /// </summary>
    public string Audience { get; set; } = "Event";
    /// <summary>
    /// Время жизни токена в минутах
    /// </summary>
    public int Expires { get; set; } = 30;
    /// <summary>
    /// Секретный ключ токена
    /// </summary>
    public string Key { get; set; } = "secret.events";

}