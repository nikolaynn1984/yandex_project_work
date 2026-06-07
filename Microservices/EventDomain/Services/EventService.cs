using Event.Domain.Interfaces;
using Event.Domain.Models;
using EventDomain.Extentions;
using EventDomain.Models;
using System.ComponentModel.DataAnnotations;

namespace Event.Domain.Services;

/// <summary>
/// Сервси событий
/// </summary>
public class EventService : IEventService
{
    private readonly List<Events> events = [];

    /// <inheritdoc/>
    public async Task<PaginatedResult> Get(string? title = null, DateTime? from = null, DateTime? to = null, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {

        var tcs = new TaskCompletionSource<PaginatedResult>(cancellationToken);

        _ = Task.Run(() =>
        {

            try
            {

                if (cancellationToken.IsCancellationRequested)
                    tcs.TrySetCanceled();

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

                tcs.TrySetResult(res);
            }
            catch(ValidationException vex)
            {
                tcs.TrySetException(vex);
            }catch(Exception ex)
            {
                tcs.TrySetException(ex);
            }

           
        }, cancellationToken);

        


        return await tcs.Task;
    }

    /// <inheritdoc/>
    public async Task<Events> Get(Guid id, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<Events>(cancellationToken);

        _ = Task.Run(() =>
        {
            var model = events.FirstOrDefault(s => s.Id == id);
            if (model == null)
            {
                tcs.TrySetException(new EventException($"Событие с идентификатором {id} не найден"));
            }
            else
            {
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
                tcs.TrySetResult(model);
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            }
        });

        

        return await tcs.Task;
    }

    /// <inheritdoc/>
    public async Task<Guid> Add(EventRequest model, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<Guid>(cancellationToken);

        _ = Task.Run(() =>
        {

            var id = Guid.NewGuid();

            if (cancellationToken.IsCancellationRequested)
                tcs.TrySetCanceled();

#pragma warning disable CS8629 // Тип значения, допускающего NULL, может быть NULL.
            events.Add(new Events(id, model.Title, model.Description, model.TotalSeats, (DateTime)model.StartAt, (DateTime)model.EndAt));
#pragma warning restore CS8629 // Тип значения, допускающего NULL, может быть NULL.

            tcs.TrySetResult(id);
        }, cancellationToken);

        return await tcs.Task;
    }

    /// <inheritdoc/>
    public async Task Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource(cancellationToken);

        _ = Task.Run(() =>
        {
            
            if(cancellationToken.IsCancellationRequested)
                tcs.TrySetCanceled();

            var model = events.FirstOrDefault(s => s.Id == id);

            if (model == null)
            {
                tcs.TrySetException(new EventException($"Событие с идентификатором {id} не найден"));
            }
            else
            {
                this.events.Remove(model);
                tcs.TrySetResult();
            }
                


            
        }, cancellationToken);

       
       await tcs.Task;
       
    }



    /// <inheritdoc/>
    public async Task Update(Guid id, EventRequest data, CancellationToken cancellationToken = default)
    {

        var tcs = new TaskCompletionSource(cancellationToken);

        _ = Task.Run(() => {

            if (cancellationToken.IsCancellationRequested)
                tcs.TrySetCanceled();

            var model = events.FirstOrDefault(s => s.Id == id);

            if (model == null)
            {
                tcs.TrySetException(new EventException($"Событие с идентификатором {id} не найден"));
            }
            else
            {
                model.Title = data.Title;
                model.Description = data.Description;
#pragma warning disable CS8629 // Тип значения, допускающего NULL, может быть NULL.
                model.StartAt = (DateTime)data.StartAt;
                model.EndAt = (DateTime)data.EndAt;
#pragma warning restore CS8629 // Тип значения, допускающего NULL, может быть NULL.

                tcs.TrySetResult();
            }





        }, cancellationToken);

       


        await tcs.Task;

    }

    

    
}
