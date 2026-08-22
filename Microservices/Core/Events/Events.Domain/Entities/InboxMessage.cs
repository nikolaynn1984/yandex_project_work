namespace Events.Domain.Entities;
/// <summary>
/// Входящее сообщение
/// </summary>
public record InboxMessage(Guid Id, DateTime ReceivedOn)
{
    /// <summary>
    /// Идентифкатор
    /// </summary>
    public Guid Id { get; init; } = Id;
    /// <summary>
    /// Дата получения
    /// </summary>
    public DateTime ReceivedOn { get; init; } = ReceivedOn;
}
