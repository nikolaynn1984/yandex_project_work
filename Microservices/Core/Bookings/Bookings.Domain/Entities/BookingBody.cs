using System.Text.Json;

namespace Bookings.Domain.Entities;

public record BookingBody(Guid BookingId, Guid EventId, int SeatCount)
{
    public Guid BookingId { get; init; } = BookingId;
    public Guid EventId { get; init; } = EventId;

    public int SeatCount { get; init; } = SeatCount;
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
