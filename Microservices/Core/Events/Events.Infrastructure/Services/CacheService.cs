using Events.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Events.Infrastructure.Services;

internal class CacheService : ICacheService
{
    private readonly IDatabase? cacheDb;
    private readonly ILogger<CacheService> logger;
         
    public CacheService(IConnectionMultiplexer connection, ILogger<CacheService> logger)
    {
        this.cacheDb = connection.GetDatabase();
        this.logger = logger;
    }

    public async Task Delete(string key)
    {
        try
        {
            if(this.cacheDb != null)
               await this.cacheDb.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Ошибка удаления кэша, ключ = {key}", key);
        }
    }

    public async Task<string?> Get(string key)
    {
        try
        {
            if (this.cacheDb == null) return null;

            var res = await this.cacheDb.StringGetAsync(key);
            if(res.HasValue == true)
            {
                return res;
            }
        }catch (Exception ex)
        {
            this.logger.LogError(ex,"Ошибка получения кэша, ключ = {key}", key);
        }

        return null;
    }

    public async Task Set(string key, string value, TimeSpan ttl)
    {
        try
        {
            if( this.cacheDb == null) return;

            await this.cacheDb.StringSetAsync(key, value, ttl);
        }catch (Exception ex)
        {
            this.logger.LogError(ex, "Ошибка записи кэша, ключ = {key}", key);
        }
    }
}
