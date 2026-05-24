namespace EventDomain.Models;

/// <summary>
/// Бронь
/// </summary>
public class Booking(Guid Id, Guid EventId)
{
    /// <summary>
    /// уникальный идентификатор брони
    /// </summary>
    public Guid Id { get; set; } = Id;
    /// <summary>
    /// идентификатор события, к которому относится бронь
    /// </summary>
    public Guid EventId {  get; set; } = EventId;
    /// <summary>
    /// Текущий статус брони
    /// </summary>
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    /// <summary>
    /// Дата и время создания брони
    /// </summary>
    public DateTime CreatedAt {  get; set; } = DateTime.Now;
    /// <summary>
    /// Дата и время обработки брони
    /// </summary>
    public DateTime? ProcessedAt {  get; set; }
}
