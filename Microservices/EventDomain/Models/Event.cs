namespace EventDomain.Models;

/// <summary>
/// Событие
/// </summary>
/// <remarks>
/// Конструктор событий
/// </remarks>
/// <param name="Id">Идентифкатор</param>
/// <param name="Title">Титл</param>
/// <param name="Description">Описание</param>
/// <param name="TotalSeats">Общее количество мест</param>
/// <param name="StartAt">Начало</param>
/// <param name="EndAt">Конец</param>
public class Event(Guid Id, string Title, string? Description, int TotalSeats, DateTime StartAt, DateTime EndAt)
{
    readonly object locked = new object();
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
    /// Общее количество мест на событии
    /// </summary>
    public int TotalSeats {  get; set; } = TotalSeats;
    /// <summary>
    /// Текущее количество свободных мест
    /// </summary>
    public int AvailableSeats {  get; private set; } = TotalSeats;
    /// <summary>
    /// Начало
    /// </summary>
    public DateTime StartAt { get; set; } = StartAt;
    /// <summary>
    /// Конец
    /// </summary>
    public DateTime EndAt { get; set; } = EndAt;

    public bool TryReserveSeats(int count = 1)
    {
        lock (locked)
        {
            if(AvailableSeats == 0 || AvailableSeats < count)
                return false;

            AvailableSeats -= count;
        }
        return true;
    }

    /// <summary>
    /// Освобождение места
    /// </summary>
    /// <param name="count">Количество место</param>
    /// <returns></returns>
    public void ReleaseSeats(int count = 1)
    {
        lock (locked)
        {
            AvailableSeats += count;

            if(AvailableSeats > TotalSeats)
                AvailableSeats = TotalSeats;
        }
    }
}

