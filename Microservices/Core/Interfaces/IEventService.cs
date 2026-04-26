using EventServer.Models;

namespace EventServer.Core.Interfaces;

/// <summary>
/// Сервис событий
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Получить полный список
    /// </summary>
    /// <returns></returns>
    List<Event> Get();
    /// <summary>
    /// Получить событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <returns></returns>
    Event? Get(int id);
    /// <summary>
    /// Добавить событие
    /// </summary>
    /// <param name="model">Объектная модель Event</param>
    void Add(EventRequest model);
    /// <summary>
    /// Обновичть событие
    /// </summary>
    /// <param name="id">Идентификатор</param>
    /// <param name="data">Объектная модель Event</param>
    bool Update(int id, EventRequest data);
    /// <summary>
    /// Удалить событие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор</param>
    bool Delete(int id);
}
