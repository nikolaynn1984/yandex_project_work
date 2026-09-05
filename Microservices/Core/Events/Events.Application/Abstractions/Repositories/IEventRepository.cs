using Events.Domain.Entities;

namespace Events.Application.Abstractions.Repositories;

/// <summary>
/// Репозиторий событий
/// </summary>
public interface IEventRepository
{
    /// <summary>
    /// Получить список событий с параметрами
    /// </summary>
    /// <param name="title">Титл</param>
    /// <param name="from">От</param>
    /// <param name="to">До</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат запроса или null</returns>
    Task<IEnumerable<Event>> Get(string? title = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    /// <summary>
    /// Получить по идентификатору
    /// </summary>
    /// <param name="Id">Идентификатор события</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Event если найдено, в простивном случае null</returns>
    Task<Event?> GetById(Guid Id,  CancellationToken cancellationToken = default);
    /// <summary>
    /// Добавить событие 
    /// </summary>
    /// <param name="item">Объектная модель</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task Add(Event item, CancellationToken cancellationToken = default);
    /// <summary>
    /// Удалить событтие
    /// </summary>
    /// <param name="item">Объектная модель</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task Delete(Event item, CancellationToken cancellationToken = default);
    /// <summary>
    /// Сохранить изменения в базе
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}