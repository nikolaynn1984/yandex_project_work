using EventDomain.DataAccess;
using EventDomain.Extentions;
using EventDomain.Models;
using EventDomain.Repository;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Testcontainers.PostgreSql;

namespace EventServiceIntegrationTests;

[Collection("Database")]
public class EventRepositoryTests : IAsyncLifetime
{

    private readonly PostgreSqlContainer container = new PostgreSqlBuilder("postgres:16-alpine")
        .Build();

    public async Task DisposeAsync()
    {
        await container.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await container.StartAsync();
    }

    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(container.GetConnectionString(), s => s.MigrationsAssembly("EventServer"))
            .Options;

        var context = new AppDbContext(options);
        context.Database.Migrate();
        return context;
    }

    private async Task ResetDatabaseAsync()
    {
        await using var context = CreateContext();
        await context.Database.ExecuteSqlRawAsync(
            "TRUNCATE TABLE bookings, events RESTART IDENTITY CASCADE");
    }
    [Fact]
    public async Task Event_GetAll_list()
    {
        // Arrange
        await ResetDatabaseAsync();
        await using var context = CreateContext();
        var repository = new EventRepository(context);
        var event1 = new Event(Guid.NewGuid(), "Test 1", "Описание 1", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "Test 1" };
        var event2 = new Event(Guid.NewGuid(), "Test 2", "Описание 2", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "Test 1" };
        var event3 = new Event(Guid.NewGuid(), "Test 3", "Описание 3", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "Test 1" };
        var event4 = new Event(Guid.NewGuid(), "Test 4", "Описание 4", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "Test 1" };
        var event5 = new Event(Guid.NewGuid(), "Test 5", "Описание 5", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "Test 1" };

        await repository.Add(event1);
        await repository.Add(event2);
        await repository.Add(event3);
        await repository.Add(event4);
        await repository.Add(event5);

        await repository.SaveChangesAsync();

        

        //Act
        var events = await repository.Get();

        // Assert
        Assert.Equal(5, events.Count());
    }

    [Fact]
    public async Task Event_GetFiltersTitle_list()
    {
        // Arrange
        await ResetDatabaseAsync();
        await using var context = CreateContext();
        var repository = new EventRepository(context);
        var event1 = new Event(Guid.NewGuid(), "Filter 1", "Описание 1", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "Filter 1" };
        var event2 = new Event(Guid.NewGuid(), "Filter 2", "Описание 2", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "Filter 1" };
        var event3 = new Event(Guid.NewGuid(), "Test 3", "Описание 3", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "Test 3" };
        var event4 = new Event(Guid.NewGuid(), "Test 4", "Описание 4", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "Test 4" };
        var event5 = new Event(Guid.NewGuid(), "Test 5", "Описание 5", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "Test 5" };

        await repository.Add(event1);
        await repository.Add(event2);
        await repository.Add(event3);
        await repository.Add(event4);
        await repository.Add(event5);

        await repository.SaveChangesAsync();

        

        //Act
        var events = await repository.Get("Filter");

        // Assert
        Assert.Equal(2, events.Count());
    }


    [Fact]
    public async Task Event_GetFiltersFromTo_list()
    {
        // Arrange
        await ResetDatabaseAsync();
        await using var context = CreateContext();
        var repository = new EventRepository(context);
        var event1 = new Event(Guid.NewGuid(), "Filter 1", "Описание 1", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 06, 11).ToUniversalTime()) { Title = "Filter 1" };
        var event2 = new Event(Guid.NewGuid(), "Filter 2", "Описание 2", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 06, 11).ToUniversalTime()) { Title = "Filter 1" };
        var event3 = new Event(Guid.NewGuid(), "Test 3", "Описание 3", 5, new DateTime(2025, 08, 11).ToUniversalTime(), new DateTime(2025, 09, 12).ToUniversalTime()) { Title = "Test 3" };
        var event4 = new Event(Guid.NewGuid(), "Test 4", "Описание 4", 5, new DateTime(2025, 08, 11).ToUniversalTime(), new DateTime(2025, 09, 12).ToUniversalTime()) { Title = "Test 4" };
        var event5 = new Event(Guid.NewGuid(), "Filter 3", "Описание 5", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 06, 11).ToUniversalTime()) { Title = "Test 5" };

        await repository.Add(event1);
        await repository.Add(event2);
        await repository.Add(event3);
        await repository.Add(event4);
        await repository.Add(event5);

        await repository.SaveChangesAsync();



        //Act
        var events = await repository.Get(null, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 06, 11).ToUniversalTime());

        // Assert
        Assert.Equal(3, events.Count());
    }

    [Fact]
    public async Task Event_Add_GetById()
    {
        // Arrange
        await ResetDatabaseAsync();
        await using var context = CreateContext();
        var id = Guid.NewGuid();
        var repository = new EventRepository(context);
        var event1 = new Event(id, "тест 1", "Описание 1", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "Тест 1" };


         await  repository.Add(event1);

        await context.SaveChangesAsync();

        

        //Act
        var item = await repository.GetById(id);

        // Assert
        Assert.Equal(id, item?.Id);
    }


    [Fact]
    public async Task Event_Update_true()
    {
        try
        {

            // Arrange
            await ResetDatabaseAsync();
            await using var context = CreateContext();
            var id = Guid.NewGuid();
            var repository = new EventRepository(context);
            var eventAddItem = new Event(id, "AddTestUpdate", "Тестовое добаление для обновления", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "AddTestUpdate" };


            await repository.Add(eventAddItem);

            await repository.SaveChangesAsync();


            var eventEditItem = await repository.GetById(id);
            // Act
            eventEditItem?.Title = "EditTestUpdate";

            await repository.SaveChangesAsync();

            var item = await repository.GetById(id);

            // Assert
            Assert.Equal("EditTestUpdate", item?.Title);

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            Assert.True(false);
        }

    }

    [Fact]
    public async Task Event_Delete_Null()
    {
        try
        {

            // Arrange
            await ResetDatabaseAsync();
            await using var context = CreateContext();
            var id = Guid.NewGuid();
            var repository = new EventRepository(context);
            var eventAddItem = new Event(id, "AddTestUpdate", "Тестовое добаление для обновления", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "AddTestUpdate" };


            await repository.Add(eventAddItem);

            await repository.SaveChangesAsync();


            var eventItem = await repository.GetById(id);
            // Act
            if(eventItem != null)
              await repository.Delete(eventItem);


            // Assert
            var exception = await Assert.ThrowsAsync<EventException>(() => repository.GetById(id));

            string message = $"Событие с идентификатором {id} не найден";
            Assert.True(!string.IsNullOrEmpty(exception.Message));

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            Assert.True(false);
        }

    }
}
