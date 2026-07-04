using EventDomain.Interfaces;
using EventDomain.Models;
using EventDomain.Extentions;
using EventDomain.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace EventDomain.Services;

/// <summary>
/// Сервси событий
/// </summary>
public class EventService : IEventService
{
    private readonly AppDbContext context;

    public EventService(AppDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResult> Get(string? title = null, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {


         cancellationToken.ThrowIfCancellationRequested();

         var events = await this.context.Events.AsNoTracking().ToListAsync(cancellationToken);

         var filters = events.Filter(title, from, to);
         var pageList = filters.Pagination(page, pageSize).ToList();

         int totalPage = filters.Count().GetTotalPages(pageSize);
         int count = filters.Count();
         var res = new PaginatedResult()
         {
             TotalItems = count,
             Items = pageList,
             TotalPages = count.GetTotalPages(pageSize),
             CurrentPage = totalPage > page ? page : totalPage,
         };

         return res;
    }

    /// <inheritdoc/>
    public async Task<Event?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var model = await this.context.Events.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (model == null)
        {
           throw new EventException($"Событие с идентификатором {id} не найден");
        }
        return model;

    }



    /// <inheritdoc/>
    public async Task<Guid> Add(EventRequest model, CancellationToken cancellationToken = default)
    {
         var id = Guid.NewGuid();

         cancellationToken.ThrowIfCancellationRequested();


#pragma warning disable CS8629 // Тип значения, допускающего NULL, может быть NULL.
        var item = new Event(id, model.Title, model.Description, model.TotalSeats, (DateTime)model.StartAt, (DateTime)model.EndAt) { Title = model.Title, };
#pragma warning restore CS8629 // Тип значения, допускающего NULL, может быть NULL.

        await this.context.Events.AddAsync(item, cancellationToken);

        await this.context.SaveChangesAsync(cancellationToken);

        return id;
    }

    /// <inheritdoc/>
    public async Task Delete(Guid id, CancellationToken cancellationToken = default)
    {

        cancellationToken.ThrowIfCancellationRequested();

        var model =  await this.context.Events.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (model == null)
        {
            throw new EventException($"Событие с идентификатором {id} не найден");
        }

#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
        this.context.Events.Remove(model);
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.


        await this.context.SaveChangesAsync();
    }



    /// <inheritdoc/>
    public async Task Update(Guid id, EventRequest data, CancellationToken cancellationToken = default)
    {


        cancellationToken.ThrowIfCancellationRequested();

        var model = await this.context.Events.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (model == null)
        {
            throw new EventException($"Событие с идентификатором {id} не найден");
        }
        
        model.Title = data.Title;
        model.Description = data.Description;
#pragma warning disable CS8629 // Тип значения, допускающего NULL, может быть NULL.
        model.StartAt = (DateTime)data.StartAt;
        model.EndAt = (DateTime)data.EndAt;
#pragma warning restore CS8629 // Тип значения, допускающего NULL, может быть NULL.

        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReleaseSeats(Guid eventId, int count = 1)
    {
        var model = await this.context.Events.FirstOrDefaultAsync(s => s.Id == eventId);
        if (model == null)
            return;

        model.ReleaseSeats(count);

        await this.context.SaveChangesAsync();
    }

    
}
