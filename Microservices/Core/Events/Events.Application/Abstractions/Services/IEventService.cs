using Events.Application.Events.DTOs;
using Events.Domain.Entities;

namespace Events.Application.Abstractions.Services;

/// <summary>
/// Сервис событий
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Получить полный список
    /// </summary>
    /// <param name="title">Наименование</param>
    /// <param name="from">Дата начала</param>
    /// <param name="to">Дата окончания</param>
    /// <param name="page">Страница которую требуется вернуть</param>
    /// <param name="pageSize">Колчество элементов в странице</param>
    /// <param name="token">Токен отмены</param>
    /// <returns></returns>
    Task<PaginatedResult> Get(string? title = null, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10, CancellationToken token = default);
    /// <summary>
    /// Получить событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Событие если найдено, в противном случае EventExeption</returns>
    Task<Event?> Get(Guid id, CancellationToken token = default);
    /// <summary>
    /// Получить топ 10 событий
    /// </summary>
    /// <param name="token"></param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Список топ событий</returns>
    Task<IReadOnlyList<Event>> GetTop(CancellationToken token = default);
    /// <summary>
    /// Добавить событие
    /// </summary>
    /// <param name="model">Объектная модель Event</param>
    /// <param name="token">Токен отмены</param>
    /// <returns>Идентификатор события</returns>
    Task<Guid> Add(EventRequest model, CancellationToken token = default);
    /// <summary>
    /// Обновичть событие
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <param name="data">Объектная модель Event</param>
    /// <param name="token">Токен отмены</param>
    Task Update(Guid id, EventRequest data, CancellationToken token = default);
    /// <summary>
    /// Удалить событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <param name="token">Токен отмены</param>
    Task Delete(Guid id, CancellationToken token = default);
}
