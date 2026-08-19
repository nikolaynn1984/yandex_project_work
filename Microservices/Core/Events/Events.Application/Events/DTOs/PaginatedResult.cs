using Events.Domain.Entities;

namespace Events.Application.Events.DTOs;

/// <summary>
/// Результат запроса
/// </summary>
public class PaginatedResult
{
    /// <summary>
    /// Общее количество
    /// </summary>
    public int TotalItems { get; set; } = 0;
    /// <summary>
    /// МСобытия
    /// </summary>
    public IEnumerable<Event> Items { get; set; } = new List<Event>();
    /// <summary>
    /// Текущая страница
    /// </summary>
    public int CurrentPage { get; set; } = 0;
    /// <summary>
    /// Общее количество страниц
    /// </summary>
    public int TotalPages { get; set; } = 0;
}
