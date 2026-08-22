namespace Events.Domain.Entities;
/// <summary>Статус </summary>
public enum OutboxStatus
{
    /// <summary>Подтверждена</summary>
    Confirmed = 1,
    /// <summary>Отклонена</summary>
    Rejected = 2,
}
