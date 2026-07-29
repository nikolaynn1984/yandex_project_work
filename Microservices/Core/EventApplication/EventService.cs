using EventApplication.Abstractions.Repositories;
using EventApplication.Abstractions.Services;
using EventApplication.Events.DTOs;
using EventDomain.Entities;
using System.ComponentModel.DataAnnotations;

namespace EventApplication;

/// <summary>
/// Сервси событий
/// </summary>
public class EventService : IEventService
{
    private readonly IEventRepository eventRepository;

    public EventService(IEventRepository eventRepository)
    {
        this.eventRepository = eventRepository;
    }

    /// <inheritdoc/>
    public async Task<PaginatedResult> Get(string? title = null, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {


        cancellationToken.ThrowIfCancellationRequested();

        var filters = await this.eventRepository.Get(title, from, to, cancellationToken);

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
        return await this.eventRepository.GetById(id, cancellationToken);

    }



    /// <inheritdoc/>
    public async Task<Guid> Add(EventRequest model, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();

        cancellationToken.ThrowIfCancellationRequested();


#pragma warning disable CS8629 // Тип значения, допускающего NULL, может быть NULL.
        var item = new Event(id, model.Title, model.Description, model.TotalSeats, (DateTime)model.StartAt, (DateTime)model.EndAt) { Title = model.Title, };
#pragma warning restore CS8629 // Тип значения, допускающего NULL, может быть NULL.

        await this.eventRepository.Add(item, cancellationToken);

        return id;
    }

    /// <inheritdoc/>
    public async Task Delete(Guid id, CancellationToken cancellationToken = default)
    {

        cancellationToken.ThrowIfCancellationRequested();

        var model = await this.eventRepository.GetById(id, cancellationToken);


#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
        await this.eventRepository.Delete(model, cancellationToken);
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.

    }



    /// <inheritdoc/>
    public async Task Update(Guid id, EventRequest data, CancellationToken cancellationToken = default)
    {


        cancellationToken.ThrowIfCancellationRequested();

        var model = await this.eventRepository.GetById(id, cancellationToken);

        if (model == null) return;

        if (model.ValivadeTotalSeat(data.TotalSeats) == false)
            throw new ValidationException("Общее количество мест не должно быть меньше забронированных");

        model.Title = data.Title;
        model.Description = data.Description;
        model.TotalSeats = data.TotalSeats;
#pragma warning disable CS8629 // Тип значения, допускающего NULL, может быть NULL.
        model.StartAt = (DateTime)data.StartAt;
        model.EndAt = (DateTime)data.EndAt;
#pragma warning restore CS8629 // Тип значения, допускающего NULL, может быть NULL.


        await this.eventRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ReleaseSeats(Guid eventId, int count = 1)
    {
        var model = await this.eventRepository.GetById(eventId);
        if (model == null)
            return;

        model.ReleaseSeats(count);

        await this.eventRepository.SaveChangesAsync();
    }


}
