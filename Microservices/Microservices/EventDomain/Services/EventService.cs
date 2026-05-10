using Event.Domain.Interfaces;
using Event.Domain.Models;

namespace Event.Domain.Services;

/// <summary>
/// Сервси событий
/// </summary>
internal class EventService : IEventService
{
    private readonly List<Events> events = [];
    private int lastIndex = 0;

    /// <inheritdoc/>
    public List<Events> Get()
    {
        return events;
    }

    /// <inheritdoc/>
    public Events? Get(int id)
    {
        return events.FirstOrDefault(s => s.Id == id); ;
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
    public bool Delete(int id)
    {
        bool result = false;

        var model = events.FirstOrDefault(s => s.Id == id);
        if (model != null)
        {
            this.events.Remove(model);
            result = true;
        }

        return result;
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
#pragma warning disable CS8629 // Тип значения, допускающего NULL, может быть NULL.
            model.StartAt = (DateTime)data.StartAt;
            model.EndAt = (DateTime)data.EndAt;
#pragma warning restore CS8629 // Тип значения, допускающего NULL, может быть NULL.

            result = true;
        }


        return result;
    }
}
