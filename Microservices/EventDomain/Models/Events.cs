namespace Event.Domain.Models;

/// <summary>
/// Событие
/// </summary>
/// <remarks>
/// Конструктор событий
/// </remarks>
/// <param name="Id">Идентифкатор</param>
/// <param name="Title">Титл</param>
/// <param name="Description">Описание</param>
/// <param name="StartAt">Начало</param>
/// <param name="EndAt">Конец</param>
public class Events(Guid Id, string Title, string? Description, DateTime StartAt, DateTime EndAt)
{

    /// <summary>
    /// Идентифкатор
    /// </summary>
    public Guid Id { get; set; } = Id;
    /// <summary>
    /// Титл
    /// </summary>
    public string Title { get; set; } = Title;
    /// <summary>
    /// Описание
    /// </summary>
    public string? Description { get; set; } = Description;
    /// <summary>
    /// Начало
    /// </summary>
    public DateTime StartAt { get; set; } = StartAt;
    /// <summary>
    /// Конец
    /// </summary>
    public DateTime EndAt { get; set; } = EndAt;
}

