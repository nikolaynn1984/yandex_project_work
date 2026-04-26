using EventServer.Core.Interfaces;
using EventServer.Models;

namespace EventServer.Core.Services;

/// <summary>
/// Сервси событий
/// </summary>
public class EventService : IEventService
{
    private readonly List<Event> events = [];
    private int lastIndex = 0;

    /// <inheritdoc/>
    public List<Event> Get()
    {
        return events;
    }

    /// <inheritdoc/>
    public Event? Get(int id)
    {
        return events.FirstOrDefault(s => s.Id == id); ;
    }

    /// <inheritdoc/>
    public void Add(EventRequest model)
    {
        lastIndex++;

        events.Add(new Event(lastIndex, model.Title, model.Description, (DateTime)model.StartAt, (DateTime)model.EndAt));

    }

    /// <inheritdoc/>
    public bool Delete(int id)
    {
        bool result = false;

        var model = events.FirstOrDefault(s => s.Id == id);
        if(model != null)
        {
            this.events.Remove(model);
            result = true;
        }

        return  result;
    }

    

    /// <inheritdoc/>
    public bool Update(int id, EventRequest data)
    {
        bool result = false;
        var model = events.FirstOrDefault(s => s.Id == id); ;
        if (model != null)
        {
            model.Title = data.Title;
            model.Description = data.Description;
            model.StartAt = (DateTime)data.StartAt;
            model.EndAt = (DateTime)data.EndAt;

            result = true;
        }
        

        return result;
    }
}
