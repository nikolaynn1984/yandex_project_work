using Bookings.Application.DTOs;
using Bookings.Domain.Entities;

namespace Bookings.Application.Abstractions.Services;
/// <summary>
/// Валидация сервиса
/// </summary>
public interface IBookingValidator
{
    /// <summary>
    /// Выполнить проверку
    /// </summary>
    /// <param name="user">Пользоваетль</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task<bool> UserSeatsCount(UserContext user, CancellationToken cancellationToken = default);
    /// <summary>
    /// Влидация отмены бронирования
    /// </summary>
    /// <param name="booking">Бронирование</param>
    /// <param name="user">Пользователь</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task CanceledValild(Booking booking, UserContext user, CancellationToken cancellationToken = default);
}
