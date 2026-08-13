namespace EventApplication.Events.DTOs;
/// <summary>
/// Контекст пользователя
/// </summary>
public class UserContext
{
    /// <summary>
    /// Идентификато
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Логин
    /// </summary>
    public string Login {  get; set; } = string.Empty;
    /// <summary>
    /// Роль пользователя
    /// </summary>
    public string Role {  get; set; } = string.Empty;
}
