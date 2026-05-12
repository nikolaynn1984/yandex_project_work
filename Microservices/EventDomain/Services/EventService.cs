using Event.Domain.Interfaces;
using Event.Domain.Models;
using EventDomain.Extentions;
using EventDomain.Models;

namespace Event.Domain.Services;

/// <summary>
/// Сервси событий
/// </summary>
public class EventService : IEventService
{
    private readonly List<Events> events = [];
    private int lastIndex = 0;

    /// <inheritdoc/>
    public PaginatedResult Get(string? title = null, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10)
    {
        var filters = events.Filter(title, from, to);
        var pageList = filters.Pagination( page, pageSize).ToList();
        return new PaginatedResult()
        {
            TotalItems = events.Count,
            Items = pageList,
            TotalPages = events.Count.GetTotalPages(pageSize),
            CurrentPage = page,
            CurrentCount = pageList.Count
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

    

    
}
