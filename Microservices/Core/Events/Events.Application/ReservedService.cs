using Events.Application.Abstractions.Repositories;
using Events.Application.Abstractions.Services;
using Events.Domain.Entities;
using Events.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Events.Application;
/// <summary>
/// Сервис резервирования мест
/// </summary>
public class ReservedService : IReservedService
{
    private readonly IInboxRepository inboxRepository;
    private readonly IEventRepository eventRepository;
    private readonly IOutboxRepository outboxRepository;

    public ReservedService(IInboxRepository inboxRepository, IEventRepository eventRepository, IOutboxRepository outboxRepository)
    {
        this.inboxRepository = inboxRepository;
        this.eventRepository = eventRepository;
        this.outboxRepository =   outboxRepository;
    }

    public async Task Execute(Guid EventId, Guid BookingId, int SeatCount, CancellationToken cancellationToken = default)
    {
        var OccurredOn = DateTime.UtcNow;


         await this.inboxRepository.Add(new InboxMessage(BookingId, OccurredOn), cancellationToken);


         var eventItem = await this.eventRepository.GetById(EventId, cancellationToken); 
         if(eventItem != null)
         {
             if (eventItem.TryValivadeStartAt() == false)
             {
                 await AddOutbox(EventId, BookingId, OccurredOn, OutboxStatus.Rejected, cancellationToken);
                 throw new ValidationException("Событие уже началось");

             }
                 

             if (eventItem.TryReserveSeats(SeatCount) == false)
             {
                 await AddOutbox(EventId, BookingId, OccurredOn, OutboxStatus.Rejected, cancellationToken);
                 throw new NoAvailableSeatsException("Свободных мест на это мероприятие нет.");
             }
                 

             
         }

         await this.eventRepository.SaveChangesAsync(cancellationToken);

         await AddOutbox(EventId, BookingId, OccurredOn, OutboxStatus.Confirmed, cancellationToken);
    }

    private async Task AddOutbox(Guid EventId, Guid BookingId, DateTime OccurredOn, OutboxStatus Status,CancellationToken cancellationToken)
    {
        var body = new OutboxBody(BookingId, EventId, OccurredOn, Status);

        var outboxMessage = new OutboxMessage(Guid.NewGuid(), OccurredOn, "ReservationProcessed", body.ToString());

        await this.outboxRepository.Add(outboxMessage, cancellationToken);
        await this.outboxRepository.SaveChangesAsync(cancellationToken);
    }
}
