namespace EventDomain;

/// <summary>Статусы брони</summary>
public enum BookingStatus
{
    /// <summary>Создана, ожидает обработки</summary>
    Pending = 1,
    /// <summary>Подтверждена</summary>
    Confirmed = 2,
    /// <summary>Отклонена</summary>
    Rejected = 3,
    /// <summary>Отмена</summary>
    Cancelled = 4,
}
