using Events.Application;
using Events.Application.Abstractions.Repositories;
using Events.Application.Abstractions.Services;
using Events.Application.Events.DTOs;
using Events.Domain.Entities;
using Events.Infrastructure.DataAccess;
using Events.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Events.Unit.Test;

public class CacheTest
{
    private readonly ServiceProvider serviceProvider;
    private readonly IServiceScope scope;
    private readonly IEventService serviceEvents;
    private readonly IEventRepository eventRepository;
    private readonly ICacheService cacheService;

    public CacheTest()
    {
        var dbName = Guid.NewGuid().ToString();
        var services = new ServiceCollection();
        services.AddDbContext<EventDbContext>(options =>
            options.UseInMemoryDatabase(dbName));

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IInboxRepository, InboxRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IReservedService, ReservedService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<ICacheService, CacheServiceTest>();

        this.serviceProvider = services.BuildServiceProvider();
        this.scope = this.serviceProvider.CreateScope();

        this.serviceEvents = this.scope.ServiceProvider.GetRequiredService<IEventService>();
        this.eventRepository = this.scope.ServiceProvider.GetRequiredService<IEventRepository>();
        this.cacheService = this.scope.ServiceProvider.GetRequiredService<ICacheService>();
    }
    [Fact]
    public async Task Event_Get_Cache()
    {
        var eventId = await this.serviceEvents.Add(new EventRequest() { Title = "Test 1", Description = "Описание 1", TotalSeats = 1, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) });
        Event item = null;
        var value = await this.cacheService.Get($"event:{eventId}");
        if(string.IsNullOrEmpty(value) == false)
        {
            item = JsonSerializer.Deserialize<Event>(value);
        }

        Assert.NotNull(item);
        Assert.True(item.Id == eventId);
    }


    [Fact]
    public async Task Event_Get_Repository()
    {
        var eventId = await this.serviceEvents.Add(new EventRequest() { Title = "Test 1", Description = "Описание 1", TotalSeats = 1, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) });
        Event itemCache = null;

        await this.cacheService.Delete($"event:{eventId}");

        var value = await this.cacheService.Get($"event:{eventId}");
        if (string.IsNullOrEmpty(value) == false)
        {
            itemCache = JsonSerializer.Deserialize<Event>(value);
        }

        var item = await this.eventRepository.GetById(eventId);


        Assert.Null(itemCache);
        Assert.NotNull(item);
        Assert.True(item.Id == eventId);
    }

    [Fact]
    public async Task Event_Update_Сonsistency()
    {
        var eventId = await this.serviceEvents.Add(new EventRequest() { Title = "Test 1", Description = "Описание 1", TotalSeats = 5, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) });
        var item = await this.eventRepository.GetById(eventId);

        await this.serviceEvents.Update(eventId, new EventRequest() { Title = "Test 1", Description = "Описание 1", TotalSeats = 3, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) });

        var item2 = await this.eventRepository.GetById(eventId);
        Event cahceItem = null;

        var value = await this.cacheService.Get($"event:{eventId}");
        if(string.IsNullOrEmpty(value) == false)
        {
            cahceItem = JsonSerializer.Deserialize<Event>(value);
        }

        Assert.True(item2.TotalSeats == 3);
        Assert.NotNull(cahceItem);
        Assert.True(item2.TotalSeats == cahceItem.TotalSeats);
    }
}
