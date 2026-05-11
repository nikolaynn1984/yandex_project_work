using Event.Domain.Models;

namespace Event.Domain.Interfaces;

/// <summary>
/// Сервис событий
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Получить полный список
    /// </summary>
    /// <returns></returns>
    List<Events> Get();
    /// <summary>
    /// Получить событие по идентификатору
    /// </summary>
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
