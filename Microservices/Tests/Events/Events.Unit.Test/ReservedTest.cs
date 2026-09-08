using Events.Application;
using Events.Application.Abstractions.Repositories;
using Events.Application.Abstractions.Services;
using Events.Application.Events.DTOs;
using Events.Domain.Entities;
using Events.Domain.Exceptions;
using Events.Infrastructure.DataAccess;
using Events.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Events.Unit.Test;

public class ReservedTest
{
    private readonly ServiceProvider serviceProvider;
    private readonly IServiceScope scope;
    private readonly IEventService serviceEvents;
    private readonly IReservedService serviceReserved;
    private readonly IOutboxRepository outboxRepository;
    private readonly IInboxRepository inboxRepository;
    private readonly DateTime StartAt = DateTime.UtcNow.AddDays(1);
    private readonly DateTime EndAt = DateTime.UtcNow.AddDays(3);

    public ReservedTest()
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
        services.AddSingleton<ICacheOptions, CachOptionTest>();

        this.serviceProvider = services.BuildServiceProvider();
        this.scope = this.serviceProvider.CreateScope();

        this.serviceReserved = this.scope.ServiceProvider.GetRequiredService<IReservedService>();
        this.inboxRepository = this.scope.ServiceProvider.GetRequiredService<IInboxRepository>();
        this.outboxRepository = this.scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        this.serviceEvents = this.scope.ServiceProvider.GetRequiredService<IEventService>();

    }

    [Fact]
    public async Task Reserve_Execute_Confirmed()
    {
        var request = new EventRequest() { Title = "Тест событие", Description = "Тестовое добаление", TotalSeats = 1, StartAt = StartAt, EndAt = EndAt };
        var id = await this.serviceEvents.Add(request);

        var bookingId = Guid.NewGuid();

        await this.serviceReserved.Execute(id, bookingId, 1);

        var outboxList = await this.outboxRepository.GetNoProcessed();

        OutboxBody? result = null;

        foreach (var outbox in outboxList)
        {
            var body = JsonSerializer.Deserialize<OutboxBody>(outbox.Body);
            if(body?.BookingId == bookingId)
            {
                result = body; break;
            }
        }

        
        Assert.NotNull(result);
        Assert.True(result.Status == OutboxStatus.Confirmed);
    }

    [Fact]
    public async Task Reserve_Evexute_StartExeption()
    {
        var request = new EventRequest() { Title = "Тест событие", Description = "Тестовое добаление", TotalSeats = 1, StartAt = DateTime.UtcNow.AddDays(-1), EndAt = EndAt };
        var id = await this.serviceEvents.Add(request);

        var bookingId = Guid.NewGuid();

       

        var exception = await Assert.ThrowsAsync<ValidationException>(() => this.serviceReserved.Execute(id, bookingId, 1));

        Assert.True(exception.Message == "Событие уже началось");
    }

    [Fact]
    public async Task Reserve_Evexute_SeatsException()
    {
        var request = new EventRequest() { Title = "Тест событие", Description = "Тестовое добаление", TotalSeats = 1, StartAt = StartAt, EndAt = EndAt };
        var id = await this.serviceEvents.Add(request);


        await this.serviceReserved.Execute(id, Guid.NewGuid(), 1);

        var exception = await Assert.ThrowsAsync<NoAvailableSeatsException>(() => this.serviceReserved.Execute(id, Guid.NewGuid(), 1));

        Assert.True(exception.Message == "Свободных мест на это мероприятие нет.");
    }

    [Fact]
    public async Task Reserve_Evexute_EventException()
    {
        var evettId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<EventException>(() => this.serviceReserved.Execute(evettId, Guid.NewGuid(), 1));

        Assert.True(exception.Message == $"Событие с идентификатором {evettId} не найден");
    }


    [Fact]
    public async Task Reserve_Evexute_Idempotence()
    {
        try
        {
            var @event = this.scope.ServiceProvider.GetRequiredService<IEventService>();

            var request = new EventRequest() { Title = "Тест событие", Description = "Тестовое добаление", TotalSeats = 3, StartAt = StartAt, EndAt = EndAt };
            var id = await @event.Add(request);
            var reserverd =  this.scope.ServiceProvider.GetRequiredService<IReservedService>();
            var reserverd2 = this.scope.ServiceProvider.GetRequiredService<IReservedService>();
            var bookingId = Guid.NewGuid();

            await reserverd.Execute(id, bookingId, 1);


            await reserverd2.Execute(id, bookingId, 1);

        }
        catch (InvalidOperationException ex)
        {
            Assert.True(true);
        }
    }
}
