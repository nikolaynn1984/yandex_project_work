using Events.Application.Abstractions.Services;
using System.Collections.Concurrent;

namespace Events.Unit.Test;

internal class CacheServiceTest : ICacheService
{
    private readonly ConcurrentDictionary<string, string> cache;

    public CacheServiceTest()
    {
        this.cache = new ConcurrentDictionary<string, string>();
    }

    public Task Delete(string key)
    {
        this.cache.TryRemove(key, out string? value);

        return Task.CompletedTask;
    }

    public Task<string?> Get(string key)
    {
        if(this.cache.TryGetValue(key, out string? value) == true)
        {
#pragma warning disable CS8619 // Допустимость значения NULL для ссылочных типов в значении не соответствует целевому типу.
            return Task.FromResult(value);
#pragma warning restore CS8619 // Допустимость значения NULL для ссылочных типов в значении не соответствует целевому типу.
        }

#pragma warning disable CS8619 // Допустимость значения NULL для ссылочных типов в значении не соответствует целевому типу.
        return Task.FromResult(string.Empty);
#pragma warning restore CS8619 // Допустимость значения NULL для ссылочных типов в значении не соответствует целевому типу.
    }

    public Task Set(string key, string value, TimeSpan ttl)
    {
        if(this.cache.TryGetValue(key, out string? res ) == true)
        {
            this.cache.TryUpdate(key, value, res);
        }
        else
        {
            this.cache.TryAdd(key, value);
        }

        return Task.CompletedTask;
    }
}
