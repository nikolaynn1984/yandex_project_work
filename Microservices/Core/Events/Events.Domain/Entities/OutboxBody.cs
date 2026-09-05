using System.Text.Json;

namespace Events.Domain.Entities;

public record OutboxBody(Guid BookingId, Guid EventId, DateTime ConfirmedAt, OutboxStatus Status)
{
    /// <summary>
    /// Идентифкатор брони
    /// </summary>
    public Guid BookingId { get; init; } = BookingId;
    /// <summary>
    /// Идентификатор события
    /// </summary>
    public Guid EventId { get; init; } = EventId;
    /// <summary>
    /// Момент подтверждения
    /// </summary>
    public DateTime ConfirmedAt { get; init; } = ConfirmedAt;
    /// <summary>
    /// Статус
    /// </summary>
    public OutboxStatus Status { get; init; } = Status;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
