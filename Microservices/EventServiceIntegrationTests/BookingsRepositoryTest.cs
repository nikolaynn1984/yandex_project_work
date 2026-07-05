using EventDomain.DataAccess;
using EventDomain.Extentions;
using EventDomain.Models;
using EventDomain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Testcontainers.PostgreSql;

namespace EventServiceIntegrationTests
{
    public class BookingsRepositoryTest : IAsyncLifetime
    {
        private readonly PostgreSqlContainer container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
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
                .UseNpgsql(container.GetConnectionString())
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private async Task ResetDatabaseAsync()
        {
            await using var context = CreateContext();
            await context.Database.ExecuteSqlRawAsync(
                "TRUNCATE TABLE bookings, events RESTART IDENTITY CASCADE");
        }

        [Fact]
        public async Task Booking_AddAndGetById_Item()
        {
            // Arrange
            await ResetDatabaseAsync();
            await using var context = CreateContext();
            var repositoryEvent = new EventRepository(context);
            var repositoryBooking = new BookingRepository(context);
            var event1 = new Event(Guid.NewGuid(), "Test 1", "Описание 1", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "Test 1" };

            

            
            await repositoryEvent.Add(event1);

            var id = Guid.NewGuid();
            var booking = new Booking(id, event1.Id);

            //Act 

            await repositoryBooking.Add(booking);

            var item = await repositoryBooking.GetById(id);

            //Assert
            Assert.NotNull(item);
            Assert.Equal(item.Id, id);
        }

        [Fact]
        public async Task Booking_Confirm_True()
        {
            // Arrange
            await ResetDatabaseAsync();
            await using var context = CreateContext();
            var repositoryEvent = new EventRepository(context);
            var repositoryBooking = new BookingRepository(context);
            var event1 = new Event(Guid.NewGuid(), "Test 1", "Описание 1", 5, new DateTime(2025, 05, 11).ToUniversalTime(), new DateTime(2025, 05, 12).ToUniversalTime()) { Title = "Test 1" };




            await repositoryEvent.Add(event1);

            var id = Guid.NewGuid();
            var booking = new Booking(id, event1.Id);

            //Act 

            await repositoryBooking.Add(booking);

            var item = await repositoryBooking.GetById(id);
            item.Confirm();
            await repositoryBooking.SaveChangesAsync();

            var itemConfirm = await repositoryBooking.GetById(id);

            //Assert
            Assert.True(itemConfirm.Status == BookingStatus.Confirmed);
        }

        [Fact]
        public async Task Booking_Add_ThrowFK()
        {
            try
            { // Arrange
                await ResetDatabaseAsync();
                await using var context = CreateContext();
                var repositoryBooking = new BookingRepository(context);



                var id = Guid.NewGuid();
                var booking = new Booking(id, Guid.NewGuid());

                //Act 


                var exception = await Assert.ThrowsAsync<DbUpdateException>(() => repositoryBooking.Add(booking));


                //Assert
                Assert.NotNull(exception);

            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
           
            
        }
    }
}
