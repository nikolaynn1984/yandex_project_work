using EventDomain.Models;

namespace EventDomain.Interfaces;

public interface IBookingRepository
{
    /// <summary>
    /// Получить бронь по идентификатору
    /// </summary>
    /// <param name="Id">Идентиифкатор</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Booking модель или null</returns>
    Task<Booking> GetById(Guid Id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Список бронирования по идентификатору события
    /// </summary>
    /// <param name="EventId">Иденнтификатор события</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список</returns>
    Task<List<Booking>> GetByEventId(Guid EventId, CancellationToken cancellationToken = default);
    /// <summary>
    /// Добавление новой брони по идентификатору события
    /// </summary>
    /// <param name="booking">Модель брони</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task Add(Booking booking, CancellationToken cancellationToken = default);
    /// <summary>
    /// Сохранить изменения
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

}
