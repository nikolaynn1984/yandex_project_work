namespace Events.Application.Abstractions.Services;
/// <summary>
/// Сервси кэширования
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Получить данные по ключу
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <returns>Значение</returns>
    Task<string?>  Get(string key);
    /// <summary>
    /// Записать данные
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="value">Значение</param>
    /// <param name="ttl">Время жизни</param>
    Task Set(string key, string value, TimeSpan ttl);
    /// <summary>
    /// Удалить кэш
    /// </summary>
    /// <param name="key">Ключ</param>
    Task Delete(string key);
}
