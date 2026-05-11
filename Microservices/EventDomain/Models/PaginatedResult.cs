using Event.Domain.Models;

namespace EventDomain.Models;

/// <summary>
/// Результат запроса
/// </summary>
public class PaginatedResult
{
    /// <summary>
    /// Общее количество
    /// </summary>
    public int TotalItems {  get; set; } = 0;
    /// <summary>
    /// МСобытия
    /// </summary>
    public IEnumerable<Events> Items { get; set; } = new List<Events>();
    /// <summary>
    /// Текущая страница
    /// </summary>
    public int CurrentPage { get; set; } = 0;
    /// <summary>
    /// Общее количество текущей страницы
    /// </summary>
    public int TotalPages { get; set; } = 0;
}
