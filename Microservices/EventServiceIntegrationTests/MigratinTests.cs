using EventDomain.DataAccess;
using EventDomain.Models;
using EventDomain.Repository;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace EventServiceIntegrationTests;

[Collection("Database")]
public class MigratinTests : IAsyncLifetime
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
    public async Task Tables_AddItems_Get()
    {
        // Arrange
        await ResetDatabaseAsync();
        await using var context = CreateContext();
        var repository = new EventRepository(context);
        var event1 = new Event(Guid.NewGuid(), "Test 1", "Описание 1", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "Test 1" };


        await repository.Add(event1);

        await repository.SaveChangesAsync();

        var repositoryBooking = new BookingRepository(context);

        await repositoryBooking.Add( new Booking(Guid.NewGuid(), event1.Id));

        //Act
        var events = await repository.Get();
        var bookings = await repositoryBooking.GetByEventId(event1.Id);
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => repositoryBooking.Add(new Booking(Guid.NewGuid(), Guid.NewGuid())));
        // Assert
        Assert.True(events?.Count() > 0);
        Assert.True(bookings?.Count() > 0);
        Assert.NotNull(exception);
    }

   
}
