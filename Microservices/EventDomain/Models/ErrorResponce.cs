namespace EventDomain.Models;

/// <summary>
/// Ответ ошибки HTTP
/// </summary>
public class ErrorResponce
{
    /// <summary>
    /// Код ответа
    /// </summary>
    public int StatusCode {  get; set; }
    /// <summary>
    /// Сообзение об ошибке
    /// </summary>
    public string? Message {  get; set; }
    /// <summary>
    /// Тип ошибки
    /// </summary>
    public string? ErrorType {  get; set; }
}
