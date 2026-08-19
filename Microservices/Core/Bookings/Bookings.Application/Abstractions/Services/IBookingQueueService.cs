
using Bookings.Domain.Entities;

namespace Bookings.Application.Abstractions.Services;

public interface IBookingQueueService
{
    /// <summary>
    /// Событие обработки
    /// </summary>
    event Action<List<Booking>>? OnNextEvent;
    /// <summary>
    /// Добавить в очередь обработки бронирования
    /// </summary>
    /// <param name="booking">Модель бронирования</param>
    public void Add(Booking booking);
    /// <summary>
    /// Получить последнее бронирование
    /// </summary>
    /// <param name="booking">Бронирование</param>
    /// <returns>true  если имеется значение, в противном случае false</returns>
    Task Next();
}
