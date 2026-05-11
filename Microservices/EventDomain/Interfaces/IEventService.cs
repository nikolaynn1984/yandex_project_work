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
    PaginatedResult Get(string? title, DateTime? from, DateTime? to, int? page = 1, int? pageSize = 10);
    /// <summary>
    /// Получить событие по идентификатору
    /// </summary>
    /// 
    /// <param name="id">Идентификатор</param>
    /// <returns></returns>
    Events Get(int id);
    /// <summary>
    /// Добавить событие
    /// </summary>
    /// <param name="model">Объектная модель Event</param>
    int Add(EventRequest model);
    /// <summary>
    /// Обновичть событие
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <param name="data">Объектная модель Event</param>
    void Update(int id, EventRequest data);
    /// <summary>
    /// Удалить событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор</param>
    void Delete(int id);
}
