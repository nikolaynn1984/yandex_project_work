using EventDomain.DataAccess;
using EventDomain.Extentions;
using EventDomain.Interfaces;
using EventDomain.Models;
using EventDomain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventServiceTests
{
    public class BookingslTest
    {
        private readonly ServiceProvider serviceProvider;
        private readonly IServiceScope scope;
        private readonly IEventService eventService;
        private readonly IBookingService bookingService;

        public BookingslTest()
        {

            var dbName = Guid.NewGuid().ToString();
            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(dbName));
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddSingleton<IBookingQueueService, BookingQueueService>();

            this.serviceProvider = services.BuildServiceProvider();
            this.scope = this.serviceProvider.CreateScope();

            this.eventService = this.scope.ServiceProvider.GetRequiredService<IEventService>();
            this.bookingService = this.scope.ServiceProvider.GetRequiredService<IBookingService>();
        }

        [Fact]
        public async Task Booking_Add_Pending()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add Pending", Description = "Описание 1", TotalSeats = 1, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) }, CancellationToken.None);
           
            
            var booking = await this.bookingService.CreateBookingAsync(eventId, CancellationToken.None);


            Assert.True(booking?.Status == BookingStatus.Pending);
        }

        [Fact]
        public async Task Booking_Add_Id()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add Id", Description = "Описание 1", TotalSeats = 2, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) }, CancellationToken.None);


            var booking1 = await this.bookingService.CreateBookingAsync(eventId, CancellationToken.None);
            var booking2 = await this.bookingService.CreateBookingAsync(eventId, CancellationToken.None);


            Assert.True(booking1?.Id != booking2?.Id);
        }

        [Fact]
        public async Task Bookink_GetById_Model()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test GetById", Description = "Описание 1", TotalSeats = 1, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) }, CancellationToken.None);
            var result = await this.bookingService.CreateBookingAsync(eventId, CancellationToken.None);
            Booking? booking = null;
            if(result != null)
            {
                booking = await this.bookingService.GetBookingByIdAsync(result.Id, CancellationToken.None);
            }

            Assert.NotNull(booking);

        }
        [Fact]
        public async Task Booking_Add_NoAvailableSeats()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add NoAvailableSeats", Description = "Описание 1", TotalSeats = 2, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) }, CancellationToken.None);


            var booking1 = await this.bookingService.CreateBookingAsync(eventId, CancellationToken.None);
            var booking2 = await this.bookingService.CreateBookingAsync(eventId, CancellationToken.None);

            var exception = await Assert.ThrowsAsync<NoAvailableSeatsException>(() => this.bookingService.CreateBookingAsync(eventId, CancellationToken.None));

            Assert.Equal($"No available seats for this event", exception.Message);
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

        [Fact]
        public async Task Booking_Add_Confirm()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add NoAvailableSeats", Description = "Описание 1", TotalSeats = 2, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) }, CancellationToken.None);


            var result = await this.bookingService.CreateBookingAsync(eventId, CancellationToken.None);
            var booking = await this.bookingService.GetBookingByIdAsync(result.Id, CancellationToken.None);
            booking.Confirm();




            Assert.True(booking.Status == BookingStatus.Confirmed);
            Assert.True(booking.ProcessedAt != null);
        }

        [Fact]
        public async Task Booking_Add_Reject()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add NoAvailableSeats", Description = "Описание 1", TotalSeats = 1, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) }, CancellationToken.None);


            var result = await this.bookingService.CreateBookingAsync(eventId, CancellationToken.None);
            Booking? booking = null;
            if (result != null)
            {
                booking = await this.bookingService.GetBookingByIdAsync(result.Id, CancellationToken.None);
                booking?.Reject();
            }


            await this.eventService.ReleaseSeats(eventId);



            var result2 = await this.bookingService.CreateBookingAsync(eventId, CancellationToken.None);

            Assert.True(booking?.Status == BookingStatus.Rejected);
            Assert.True(booking.ProcessedAt != null);
        }

        [Fact]
        public async Task Booking_Add_Сompetition()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add NoAvailableSeats", Description = "Описание 1", TotalSeats = 5, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) }, CancellationToken.None);
            var eventItem = await this.eventService.GetAsync(eventId, CancellationToken.None);

            var tasks = new Task[20];
            int reserveCount = 0;
            int noValidCount = 0;
             

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {

                     var res = eventItem.TryReserveSeats();
                     if (res == true)
                     {
                         Interlocked.Increment(ref reserveCount);
                     }
                     else
                     {
                         Interlocked.Increment(ref noValidCount);
                     }
                });
            }

            await Task.WhenAll(tasks);

            Assert.True(reserveCount == 5);
            Assert.True(noValidCount == 15);
            Assert.True(eventItem.AvailableSeats == 0);
        }
    }

}
