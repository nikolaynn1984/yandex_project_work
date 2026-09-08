using Events.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Events.Infrastructure.Services;

internal class CacheOption : ICacheOptions
{

    public CacheOption(IConfiguration configuration)
    {
        this.Top10TTL = GetOptionOdDefault(configuration["Redis:Top10TTL"], 10);
        this.EventIdTTL = GetOptionOdDefault(configuration["Redis:EventIdTTL"], 5);
    }

    private long GetOptionOdDefault(string? value, long defaultValue)
    {
        if(long.TryParse(value, out var od))
        {
            return od;
        }
        return defaultValue;
    }

    public long Top10TTL { get; init; }
    public long EventIdTTL { get; init; }
}
