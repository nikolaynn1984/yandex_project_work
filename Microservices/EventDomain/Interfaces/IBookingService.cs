using EventDomain.Models;

namespace EventDomain.Interfaces;

/// <summary>
/// Бронирование бронирований
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Создание брони для указанного события
    /// </summary>
    /// <param name="eventId">Идентификатор события</param>
    /// <returns></returns>
    Task<AddBookingResult> CreateBookingAsync(Guid eventId, CancellationToken cancellationToken = default);
    /// <summary>
    /// Получение брони по идентификатору
    /// </summary>
    /// <param name="bookingId">Идентификатор брони</param>
    /// <returns>Объектная модель Booking</returns>
    Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
}
