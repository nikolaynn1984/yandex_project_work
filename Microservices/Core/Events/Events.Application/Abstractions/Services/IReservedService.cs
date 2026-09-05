namespace Events.Application.Abstractions.Services;

/// <summary>
/// Сервис резервирования мест
/// </summary>
public interface IReservedService
{
    /// <summary>
    /// Выполнить резерврование
    /// </summary>
    /// <param name="EventId">Идентификатор события</param>
    /// <param name="BookingId">Идентификатор бронирования</param>
    /// <param name="SeatCount">Количество мест</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <exception cref="InboxUniqueException">Исключение нарушени уникальности иднетификатра бронирования</exception>
    Task Execute(Guid EventId, Guid BookingId, int SeatCount, CancellationToken cancellationToken = default);
}
