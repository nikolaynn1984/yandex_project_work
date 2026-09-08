using Events.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Events.Unit.Test;

internal class CachOptionTest : ICacheOptions
{
    public long Top10TTL { get; init; } = 10;
    public long EventIdTTL { get; init; } = 5;
}
