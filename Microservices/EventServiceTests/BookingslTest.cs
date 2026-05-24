using Event.Domain.Interfaces;
using Event.Domain.Models;
using Event.Domain.Services;
using EventDomain.Extentions;
using EventDomain.Interfaces;
using EventDomain.Models;
using EventDomain.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventServiceTests
{
    public class BookingslTest
    {
        private readonly IEventService eventService;
        private readonly IBookingService bookingService;

        public BookingslTest()
        {
            this.eventService = new EventService();
            this.bookingService = new BookingService(this.eventService, new BookingQueueService());
        }

        [Fact]
        public async Task Booking_Add_Pending()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add Pending", Description = "Описание 1", StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) }, CancellationToken.None);
           
            
            var booking = await this.bookingService.CreateBookingAsync(eventId, CancellationToken.None);


            Assert.True(booking.Status == BookingStatus.Pending);
        }

        [Fact]
        public async Task Booking_Add_Id()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add Id", Description = "Описание 1", StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) }, CancellationToken.None);


            var booking1 = await this.bookingService.CreateBookingAsync(eventId, CancellationToken.None);
            var booking2 = await this.bookingService.CreateBookingAsync(eventId, CancellationToken.None);


            Assert.True(booking1.Id != booking2.Id);
        }

        [Fact]
        public async Task Bookink_GetById_Model()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test GetById", Description = "Описание 1", StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) }, CancellationToken.None);
            var result = await this.bookingService.CreateBookingAsync(eventId, CancellationToken.None);


            var booking = await this.bookingService.GetBookingByIdAsync(result.Id, CancellationToken.None);


            Assert.NotNull(booking);

        }

        [Fact]
        public async Task Booking_AddNoEvemtId_Throw()
        {
            var id = Guid.NewGuid();

            var exception = await Assert.ThrowsAsync<EventException>(() => this.bookingService.CreateBookingAsync(id, CancellationToken.None));


            Assert.Equal($"Событие с идентификатором {id} не найден", exception.Message);
        }

        [Fact]
        public async Task Booking_AddNoId_Throw()
        {
            var id = Guid.NewGuid();

            var exception = await Assert.ThrowsAsync<EventException>(() => this.bookingService.GetBookingByIdAsync(id, CancellationToken.None));


            Assert.Equal($"Бронирование с идентификатором {id} не найден", exception.Message);
        }
    }
}
