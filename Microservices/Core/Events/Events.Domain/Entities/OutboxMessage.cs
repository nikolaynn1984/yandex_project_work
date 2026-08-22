namespace Events.Domain.Entities;
/// <summary>
/// Исходящее сообщение
/// </summary>
public class OutboxMessage
{
    /// <summary>
    /// Идентифкатор брони
    /// </summary>
    public Guid BookingId { get; set; }
    /// <summary>
    /// Идентификатор события
    /// </summary>
    public Guid EventId { get; set; }
    /// <summary>
    /// Момент подтверждения
    /// </summary>
    public DateTime ConfirmedAt { get; set; }
    /// <summary>
    /// Статус
    /// </summary>
    public OutboxStatus Status { get; set; }
}
