namespace EventDomain.Models;

/// <summary>
/// Результат ответа добавления дронирования
/// </summary>
public class AddBookingResult(Guid Id)
{
    /// <summary>
    /// Идентификатор бронирования
    /// </summary>
    public Guid Id { get; set; } = Id;
}
