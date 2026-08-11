using EventDomain.Entities;
using EventInfrastructure.DataAccess;
using EventInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace EventServiceIntegrationTests;

[Collection("Database")]
public class MigratinTests : DataContainer
{

    
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
