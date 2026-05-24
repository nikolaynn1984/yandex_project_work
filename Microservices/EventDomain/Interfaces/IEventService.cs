using Event.Domain.Models;
using EventDomain.Models;

namespace Event.Domain.Interfaces;

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
    /// <returns></returns>
    PaginatedResult Get(string? title = null, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10, CancellationToken token = default);
    /// <summary>
    /// Получить событие по идентификатору
    /// </summary>
    /// 
    /// <param name="id">Идентификатор</param>
    /// <returns></returns>
    Events Get(Guid id, CancellationToken token = default);
    /// <summary>
    /// Добавить событие
    /// </summary>
    /// <param name="model">Объектная модель Event</param>
    Guid Add(EventRequest model, CancellationToken token = default);
    /// <summary>
    /// Обновичть событие
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <param name="data">Объектная модель Event</param>
    void Update(Guid id, EventRequest data, CancellationToken token = default);
    /// <summary>
    /// Удалить событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор</param>
    void Delete(Guid id, CancellationToken token = default);
}
