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

        int totalPage = filters.Count().GetTotalPages(pageSize);
        int count = filters.Count();
        return new PaginatedResult()
        {
            TotalItems = count,
            Items = pageList,
            TotalPages = count.GetTotalPages(pageSize),
            CurrentPage = totalPage > page ? page : totalPage ,
        };
    }

    /// <inheritdoc/>
    public Events Get(Guid id)
    {
        var model = events.FirstOrDefault(s => s.Id == id);
        if (model == null)
            throw new EventException($"Событие с идентификатором {id} не найден");

        return model;
    }

    /// <inheritdoc/>
    public Guid Add(EventRequest model)
    {
        var id = Guid.NewGuid();

#pragma warning disable CS8629 // Тип значения, допускающего NULL, может быть NULL.
        events.Add(new Events(id, model.Title, model.Description, (DateTime)model.StartAt, (DateTime)model.EndAt));
#pragma warning restore CS8629 // Тип значения, допускающего NULL, может быть NULL.
        return id;
    }

    /// <inheritdoc/>
    public void Delete(Guid id)
    {
        var model = events.FirstOrDefault(s => s.Id == id);

        if (model == null) 
            throw new EventException($"Событие с идентификатором {id} не найден");


        this.events.Remove(model);
       
       
    }



    /// <inheritdoc/>
    public void Update(Guid id, EventRequest data)
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
