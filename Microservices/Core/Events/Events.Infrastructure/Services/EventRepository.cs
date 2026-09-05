using Events.Application.Abstractions.Repositories;
using Events.Application.Events.DTOs;
using Events.Domain.Entities;
using Events.Domain.Exceptions;
using Events.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure.Services;

public class EventRepository : IEventRepository
{
    private readonly EventDbContext context;

    public EventRepository(EventDbContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<Event>> Get(string? title = null, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        return await this.context.Events.AsNoTracking()
            .Filter(title, from, to)
            .ToListAsync(cancellationToken);
    }

    public async Task<Event?> GetById(Guid Id, CancellationToken cancellationToken = default)
    {
        var result = await this.context.Events.FirstOrDefaultAsync(s => s.Id == Id, cancellationToken);
        if (result == null)
        {
            throw new EventException($"Событие с идентификатором {Id} не найден");
        }

        return result;
    }

    public async Task<IReadOnlyList<Event>> GetTop(int count, CancellationToken cancellationToken = default)
    {
        return await this.context.Events.AsNoTracking()
            .Select(e => new
            {
                Event = e,
                OccupancyRate = (e.TotalSeats - e.AvailableSeats) / (double)e.TotalSeats
            })
            .OrderByDescending( x=> x.OccupancyRate )
            .Take(count)
            .Select(x => x.Event).ToListAsync(cancellationToken);
    }

    public async Task Add(Event item, CancellationToken cancellationToken = default)
    {
        await this.context.Events.AddAsync(item, cancellationToken);

        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(Event item, CancellationToken cancellationToken = default)
    {
        this.context.Events.Remove(item);

        await this.context.SaveChangesAsync(cancellationToken);
    }



    public async Task Update(Guid id, EventRequest item, CancellationToken cancellationToken = default)
    {
        var model = await this.GetById(id, cancellationToken);

        model?.Title = item.Title;
        model?.Description = item.Description;
#pragma warning disable CS8629 // Тип значения, допускающего NULL, может быть NULL.
        model?.StartAt = (DateTime)item.StartAt;
        model?.EndAt = (DateTime)item.EndAt;
#pragma warning restore CS8629 // Тип значения, допускающего NULL, может быть NULL.

        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await this.context.SaveChangesAsync(cancellationToken);
    }

    
}
