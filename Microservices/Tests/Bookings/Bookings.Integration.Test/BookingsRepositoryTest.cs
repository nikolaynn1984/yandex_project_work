using Bookings.Domain;
using Bookings.Domain.Entities;
using Bookings.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Integration.Test
{
    [Collection("Database")]
    public class BookingsRepositoryTest : DataContainer
    {

        

        [Fact]
        public async Task Booking_AddAndGetById_Item()
        {
            // Arrange
            await ResetDatabaseAsync();
            await using var context = CreateContext();
            var repositoryBooking = new BookingRepository(context);

            var eventId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var booking = new Booking(id, eventId);

            //Act 

            await repositoryBooking.Add(booking);

            var item = await repositoryBooking.GetById(id);

            //Assert
            Assert.NotNull(item);
            Assert.Equal(item.Id, id);
        }


        [Fact]
        public async Task Booking_AddAndGetByEventId_Bokkings()
        {
            // Arrange
            await ResetDatabaseAsync();
            await using var context = CreateContext();
            var repositoryBooking = new BookingRepository(context);

            var eventId = Guid.NewGuid();
            await repositoryBooking.Add(new Booking(Guid.NewGuid(), eventId));
            await repositoryBooking.Add(new Booking(Guid.NewGuid(), eventId));
            //Act 



            var bookings = await repositoryBooking.GetByEventId(eventId);

            //Assert
            Assert.NotNull(bookings);
            Assert.True(bookings.Count == 2);
        }

        [Fact]
        public async Task Booking_Confirm_True()
        {
            // Arrange
            await ResetDatabaseAsync();
            await using var context = CreateContext();
            var repositoryBooking = new BookingRepository(context);
            var eventId = Guid.NewGuid();

            var id = Guid.NewGuid();
            var booking = new Booking(id, eventId);

            //Act 

            await repositoryBooking.Add(booking);

            var item = await repositoryBooking.GetById(id);
            item.Confirm();
            await repositoryBooking.SaveChangesAsync();

            var itemConfirm = await repositoryBooking.GetById(id);

            //Assert
            Assert.True(itemConfirm.Status == BookingStatus.Confirmed);
        }

    }
}
