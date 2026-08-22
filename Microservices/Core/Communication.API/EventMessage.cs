namespace Communication.API;
/// <summary>
/// Событие обработки брони событием
/// </summary>
public class EventMessage
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
    public MessageStatus Status { get; set; }
}
