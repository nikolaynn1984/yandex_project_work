using Events.Domain.Entities;

namespace Events.Application.Abstractions.Repositories;

public interface IInboxRepository
{
    /// <summary>
    /// Добавить сообщение
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task Add(InboxMessage message, CancellationToken cancellationToken = default);
    /// <summary>
    /// Сохранить изменения
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
