namespace EventDomain.Models;

/// <summary>
/// Бронь
/// </summary>
public class Booking(Guid Id, Guid EventId)
{
    /// <summary>
    /// уникальный идентификатор брони
    /// </summary>
    public Guid Id { get; init; } = Id;
    /// <summary>
    /// идентификатор события, к которому относится бронь
    /// </summary>
    public Guid EventId {  get; init; } = EventId;
    /// <summary>
    /// Текущий статус брони
    /// </summary>
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    /// <summary>
    /// Дата и время создания брони
    /// </summary>
    public DateTime CreatedAt {  get; init; } = DateTime.Now;
    /// <summary>
    /// Дата и время обработки брони
    /// </summary>
    public DateTime? ProcessedAt {  get; set; }
}
