using Bookings.Domain;

namespace Bookings.Application.DTOs;

/// <summary>
/// Результат ответа добавления дронирования
/// </summary>
public class AddBookingResult(Guid Id, Guid EventId, BookingStatus Status)
{
    /// <summary>
    /// Идентификатор бронирования
    /// </summary>
    public Guid Id { get; set; } = Id;
    /// <summary>
    /// Идентификатор события
    /// </summary>
    public Guid EventId { get; set; } = EventId;
    /// <summary>
    /// Статус
    /// </summary>
    public BookingStatus Status { get; set; } = Status;
}
