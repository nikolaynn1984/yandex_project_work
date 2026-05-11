using Event.Domain.Interfaces;
using Event.Domain.Models;
using EventDomain.Extentions;
using EventDomain.Models;

namespace Event.Domain.Services;

/// <summary>
/// Сервси событий
/// </summary>
internal class EventService : IEventService
{
    private readonly List<Events> events = [];
    private int lastIndex = 0;

    /// <inheritdoc/>
    public PaginatedResult Get(string? title, DateTime? from, DateTime? to, int page = 1, int pageSize = 10)
    {
        var pageList = Pagination(events, page, pageSize);
        var filters = Filter(pageList, title, from, to).ToList();
        return new PaginatedResult()
        {
            TotalItems = events.Count,
            Items = filters,
            TotalPages = filters.Count,
            CurrentPage = page
        };
    }

    /// <inheritdoc/>
    public Events Get(int id)
    {
        var model = events.FirstOrDefault(s => s.Id == id);
        if (model == null)
            throw new EventException($"Событие с идентификатором {id} не найден");

        return model;
    }

    /// <inheritdoc/>
    public int Add(EventRequest model)
    {
        lastIndex++;

#pragma warning disable CS8629 // Тип значения, допускающего NULL, может быть NULL.
        events.Add(new Events(lastIndex, model.Title, model.Description, (DateTime)model.StartAt, (DateTime)model.EndAt));
#pragma warning restore CS8629 // Тип значения, допускающего NULL, может быть NULL.
        return lastIndex;
    }

    /// <inheritdoc/>
    public void Delete(int id)
    {
        var model = events.FirstOrDefault(s => s.Id == id);

        if (model == null) 
            throw new EventException($"Событие с идентификатором {id} не найден");


        this.events.Remove(model);
       
       
    }



    /// <inheritdoc/>
    public void Update(int id, EventRequest data)
    {
        var model = events.FirstOrDefault(s => s.Id == id);

        if (model == null) 
            throw new EventException($"Событие с идентификатором {id} не найден");


        model.Title = data.Title;
        model.Description = data.Description;
#pragma warning disable CS8629 // Тип значения, допускающего NULL, может быть NULL.
        model.StartAt = (DateTime)data.StartAt;
        model.EndAt = (DateTime)data.EndAt;
#pragma warning restore CS8629 // Тип значения, допускающего NULL, может быть NULL.

    }

    /// <summary>
    /// Получить спискок с пагинацией
    /// </summary>
    /// <param name="events">Список событий</param>
    /// <param name="page">Страница</param>
    /// <param name="pageSize">Количество записей в странице</param>
    /// <returns>Список</returns>
    private IEnumerable<Events> Pagination(IEnumerable<Events> events, int page, int pageSize)
    {
        return events.Skip((page - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Фильтр списка
    /// </summary>
    /// <param name="events">Список событий</param>
    /// <param name="title">Наименование</param>
    /// <param name="from">Дата начала</param>
    /// <param name="to">Дата окончания</param>
    /// <returns>Список</returns>
    private IEnumerable<Events> Filter(IEnumerable<Events> events, string? title, DateTime? from,  DateTime? to)
    {

        var result = events;

        if(!string.IsNullOrEmpty(title))
            result = events.Where(s => s.Title == title);

        if(from != null)
            result = events.Where(s => s.StartAt >= from);

        if(to != null)
            result = events.Where(s => s.EndAt <= to);


        return result;
    }
}
