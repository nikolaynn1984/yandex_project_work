namespace EventDomain.Models;

/// <summary>
/// Событие
/// </summary>
public class Event
{
    readonly object locked = new object();
    /// <summary>
    /// Конструктор событий
    /// </summary>
    private Event() { Title = null!; }

    /// <summary>
    /// Конструктор событий
    /// </summary>
    /// <param name="Id">Идентифкатор</param>
    /// <param name="Title">Титл</param>
    /// <param name="Description">Описание</param>
    /// <param name="TotalSeats">Общее количество мест</param>
    /// <param name="StartAt">Начало</param>
    /// <param name="EndAt">Конец</param>
    public Event(Guid Id, string Title, string? Description, int TotalSeats, DateTime StartAt, DateTime EndAt)
    {

        this.Id = Id;
        this.Title = Title;
        this.Description = Description;
        this.TotalSeats = TotalSeats;
        this.AvailableSeats = TotalSeats;
        this.StartAt = StartAt;
        this.EndAt = EndAt;
    }

    
    /// <summary>
    /// Идентифкатор
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Титл
    /// </summary>
    public required string Title { get; set; }
    /// <summary>
    /// Описание
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// Общее количество мест на событии
    /// </summary>
    public int TotalSeats {  get; set; }
    /// <summary>
    /// Текущее количество свободных мест
    /// </summary>
    public int AvailableSeats {  get; private set; }
    /// <summary>
    /// Начало
    /// </summary>
    public DateTime StartAt { get; set; }
    /// <summary>
    /// Конец
    /// </summary>
    public DateTime EndAt { get; set; }
    /// <summary>
    /// Брони
    /// </summary>
    public ICollection<Booking> Bookings { get; private set; } = [];

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

