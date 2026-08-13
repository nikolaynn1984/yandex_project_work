namespace EventDomain.Entities;

/// <summary>Бронь</summary>
public class Booking
{

    private Booking() { }

    public Booking(Guid Id, Guid EventId)
    {
        this.Id = Id;
        this.EventId = EventId;
        this.ProcessedAt = null;
    }


    /// <summary>уникальный идентификатор брони</summary>
    public Guid Id { get; internal set; }
    /// <summary>идентификатор события, к которому относится бронь</summary>
    public Guid EventId { get; internal set; }
    /// <summary>Текущий статус брони</summary>
    public BookingStatus Status { get; internal set; } = BookingStatus.Pending;
    /// <summary>Дата и время создания брони</summary>
    public DateTime CreatedAt { get; internal set; } = DateTime.UtcNow;
    /// <summary>Дата и время обработки брони</summary>
    public DateTime? ProcessedAt { get; set; }
    /// <summary>Событие</summary>
    public Event? Event { get; set; }
    /// <summary>Идентификатор пользователя</summary>
    public Guid UserId { get; set; }

    /// <summary>Подтверждение брони /summary>
    public void Confirm()
    {
        this.Status = BookingStatus.Confirmed;
        this.ProcessedAt = DateTime.UtcNow;
    }

    /// <summary>Отклонить бронь</summary>
    public void Reject()
    {
        this.Status = BookingStatus.Rejected;
        this.ProcessedAt = DateTime.UtcNow;
    }

    /// <summary>Отменить бронь</summary>
    public void Cancelled()
    {
        this.Status = BookingStatus.Cancelled;
        this.ProcessedAt = DateTime.UtcNow;
    }
}
