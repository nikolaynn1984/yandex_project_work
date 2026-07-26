using System;
using System.Collections.Generic;
using System.Text;

namespace EventDomain;

/// <summary>
/// Статусы брони
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Бронь создана, ожидает обработки
    /// </summary>
    Pending = 1,
    /// <summary>
    /// Бронь подтверждена
    /// </summary>
    Confirmed = 2,
    /// <summary>
    /// Бронь отклонена
    /// </summary>
    Rejected = 3
}
