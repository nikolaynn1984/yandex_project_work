using EventServer.Core.Interfaces;
using EventServer.Models;

namespace EventServer.Core.Services;

public class EventService : IEventService
{
    private readonly List<Event> events = [];



    public List<Event> Get()
    {
        return events;
    }

    public Event Get(int id)
    {
        return events[id];
    }
    public void Add(Event model)
    {
        events.Add(model);
    }

    public bool Delete(int id)
    {
        bool result = false;

        var model = events[id];
        if(model != null)
        {
            this.events.Remove(model);
            result = true;
        }

        return  result;
    }

    

    public bool Update(int id, UpdateRequest data)
    {
        bool result = false;
        var model = events[id];
        if (model != null)
        {
            model.Title = data.Title;
            model.Description = data.Description;
            model.StartAt = data.StartAt;
            model.EndAt = data.EndAt;

            result = true;
        }
        

        return result;
    }
}
