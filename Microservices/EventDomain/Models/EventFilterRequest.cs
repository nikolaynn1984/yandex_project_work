namespace EventDomain.Models;
/// <summary>
/// Запрос фильтрации событий
/// </summary>
public class EventFilterRequest
{
    /// <summary>
    /// Название
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// События, которые начинаются не раньше указанной даты
    /// </summary>
    public DateTime? From { get; set; }
    /// <summary>
    /// События, которые заканчиваются не позже указанной даты.
    /// </summary>
    public DateTime? To { get; set; }
}
