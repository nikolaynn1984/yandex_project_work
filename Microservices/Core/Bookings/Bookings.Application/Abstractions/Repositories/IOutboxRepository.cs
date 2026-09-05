using Bookings.Domain.Entities;

namespace Bookings.Application.Abstractions.Repositories;
/// <summary>
/// Репозиторий исходящих сообщений
/// </summary>
public interface IOutboxRepository
{
    /// <summary>
    /// Добавить сообщение
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task Add(OutboxMessage message, CancellationToken cancellationToken = default);
    /// <summary>
    /// Получить список не обработанных сообщений
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список</returns>
    Task<IReadOnlyList<OutboxMessage>> GetNoProcessed(CancellationToken cancellationToken = default);
    /// <summary>
    /// Сохранить изменения
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
