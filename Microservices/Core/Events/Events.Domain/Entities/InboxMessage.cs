namespace Events.Domain.Entities;
/// <summary>
/// Входящее сообщение
/// </summary>
public class InboxMessage
{
    /// <summary>
    /// Идентифкатор
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Дата получения
    /// </summary>
    public DateTime ReceivedOn { get; set; }
}
