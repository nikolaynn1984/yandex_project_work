namespace Communication.API;
/// <summary>
/// Сообщение добавления бронирования
/// </summary>
public class BookingMessage
{
    /// <summary>
    /// Идентификатор брони
    /// </summary>
    public Guid BookingId {  get; set; }
    /// <summary>
    /// Идентификатор события
    /// </summary>
    public Guid EventId {  get; set; }
    /// <summary>
    /// Количество мест
    /// </summary>
    public int SeatCount {  get; set; }
}
