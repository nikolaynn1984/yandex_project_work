namespace Events.Application.Abstractions.Services;
/// <summary>
/// Параметры кэширования
/// </summary>
public interface ICacheOptions
{
    public long Top10TTL {  get; init; }
    public long EventIdTTL { get; init; }
}
