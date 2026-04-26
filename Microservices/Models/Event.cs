namespace EventServer.Models;

/// <summary>
    /// Событие
    /// </summary>
public class Event
{
    /// <summary>
        /// Идентифкатор
        /// </summary>
    public required int Id {  get; set; }
    /// <summary>
        /// Титл
        /// </summary>
    public required string Title {  get; set; }
    /// <summary>
        /// Описание
        /// </summary>
    public string? Description { get; set; }
    /// <summary>
        /// Начало
        /// </summary>
    public DateTime StartAt { get; set; }
    /// <summary>
        /// Конец
        /// </summary>
    public DateTime EndAt { get; set; }
}

