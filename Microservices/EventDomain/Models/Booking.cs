namespace EventDomain.Models;

/// <summary>
/// Бронь
/// </summary>
public class Booking
{

    private Booking() { }

    public Booking(Guid Id, Guid EventId)
    {
        this.Id = Id;
        this.EventId = EventId;
    }


    /// <summary>
    /// уникальный идентификатор брони
    /// </summary>
    public Guid Id { get; init; }
    /// <summary>
    /// идентификатор события, к которому относится бронь
    /// </summary>
    public Guid EventId {  get; init; }
    /// <summary>
    /// Текущий статус брони
    /// </summary>
    public BookingStatus Status { get; private set; } = BookingStatus.Pending;
    /// <summary>
    /// Дата и время создания брони
    /// </summary>
    public DateTime CreatedAt {  get; init; } = DateTime.Now;
    /// <summary>
    /// Дата и время обработки брони
    /// </summary>
    public DateTime? ProcessedAt {  get; set; }

    internal Event? Event { get; private set; }

    public void Confirm()
    {
        this.Status = BookingStatus.Confirmed;
        this.ProcessedAt = DateTime.Now;
    }

    public void Reject()
    {
        this.Status = BookingStatus.Rejected;
        this.ProcessedAt = DateTime.Now;
    }
}
