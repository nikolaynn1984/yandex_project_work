using EventApplication.Events.DTOs;
using EventDomain.Entities;

namespace EventApplication.Abstractions.Services;
/// <summary>
/// Валидация сервиса
/// </summary>
public interface IBookingValidator
{
    /// <summary>
    /// Выполнить проверку
    /// </summary>
    /// <param name="eventId">Идентфиикатор события</param>
    /// <param name="user">Пользоваетль</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<bool> UserSeatsCount(Guid eventId, UserContext user, CancellationToken cancellationToken = default);
    /// <summary>
    /// Валидация события
    /// </summary>
    /// <param name="eventId">Идентификатор события</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Событие</returns>
    Task<Event?> EventHandler(Guid eventId, CancellationToken cancellationToken = default);
    /// <summary>
    /// Влидация отмены бронирования
    /// </summary>
    /// <param name="booking">Бронирование</param>
    /// <param name="user">Пользователь</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task CanceledValild(Booking booking, UserContext user, CancellationToken cancellationToken = default);
}
