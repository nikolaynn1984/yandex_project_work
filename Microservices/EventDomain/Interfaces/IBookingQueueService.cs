using EventDomain.Models;

namespace EventDomain.Interfaces;

public interface IBookingQueueService
{
    /// <summary>
    /// Добавить в очередь обработки бронирования
    /// </summary>
    /// <param name="booking">Модель бронирования</param>
    public void Enqueue(Booking booking);
    /// <summary>
    /// Получить последнее бронирование
    /// </summary>
    /// <param name="booking">Бронирование</param>
    /// <returns>true  если имеется значение, в противном случае false</returns>
    bool TryDequeue(out Booking booking);
}
