using Events.Application.Abstractions.Repositories;
using Events.Application.Abstractions.Services;
using Events.Application.Events.DTOs;
using Events.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Events.Application;

/// <summary>
/// Сервси событий
/// </summary>
public class EventService : IEventService
{
    private readonly IEventRepository eventRepository;
    private readonly ICacheService cacheService;
    private readonly string topKey;

    public EventService(IEventRepository eventRepository, ICacheService cacheService)
    {
        this.eventRepository = eventRepository;
        this.cacheService = cacheService;
        this.topKey = "events:top10";
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
    public async Task<IReadOnlyList<Event>> GetTop(CancellationToken token = default)
    {
        //Смотрим в кэш
        var topEvents = await this.cacheService.Get(topKey);
        if(string.IsNullOrEmpty(topEvents) == false)
        {
#pragma warning disable CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
            return JsonSerializer.Deserialize<List<Event>>(topEvents);
#pragma warning restore CS8603 // Возможно, возврат ссылки, допускающей значение NULL.
        }

        //Получаем с базы
        var topList = await this.eventRepository.GetTop(10,token);
        if(topList.Count > 0)
        {
            //Кладем в кэш если записей больше 0
            await this.cacheService.Set(topKey, JsonSerializer.Serialize(topList), TimeSpan.FromMinutes(10));
        }

        return topList;
    }

    /// <inheritdoc/>
    public async Task<Event?> Get(Guid id, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"event:{id}";

        //Проверяем в кэш, есть есть возвращаем
        var value = await this.cacheService.Get(cacheKey);
        if(string.IsNullOrEmpty(value) == false)
        {
            return JsonSerializer.Deserialize<Event>(value);
        }

        //Получаем с базы
        var @event = await this.eventRepository.GetById(id, cancellationToken);
        if (@event == null) return null;

        //Кладем в кэш
        await this.cacheService.Set(cacheKey, JsonSerializer.Serialize( @event), TimeSpan.FromMinutes(5));



        return @event;

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

        await this.cacheService.Set($"event:{id}", JsonSerializer.Serialize(item), TimeSpan.FromMinutes(5));

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

        await this.cacheService.Delete($"event:{id}");
        await this.cacheService.Delete(topKey);

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


        await this.cacheService.Set($"event:{id}", JsonSerializer.Serialize( model), TimeSpan.FromMinutes(5));
    }


    
}
