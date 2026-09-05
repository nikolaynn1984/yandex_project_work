namespace Bookings.Domain.Entities;
/// <summary>
/// Исходящее сообщение
/// </summary>
/// <param name="Id">Идентификатор</param>
/// <param name="OccurredOn">Дата события</param>
/// <param name="Type">Тип</param>
/// <param name="Body">Сообщение</param>
/// <param name="IsProcessed">Обработано</param>
public record OutboxMessage(Guid Id, DateTime OccurredOn, string Type, string Body, bool IsProcessed = false)
{
    public Guid Id { get; init; } = Id;
    public DateTime OccurredOn { get; init; } = OccurredOn;
    public string Type { get; init; } = Type;
    public string Body { get; init; } = Body;
    public bool IsProcessed { get; set; } = IsProcessed;

}
