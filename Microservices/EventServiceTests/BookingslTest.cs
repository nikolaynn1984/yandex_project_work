using Account.Domain.Entities;
using EventApplication;
using EventApplication.Abstractions.Repositories;
using EventApplication.Abstractions.Services;
using EventApplication.Events.DTOs;
using EventDomain;
using EventDomain.Entities;
using EventDomain.Exceptions;
using EventInfrastructure.DataAccess;
using EventInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

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
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBookingValidator, BookingValidator>();
            services.AddSingleton<IBookingQueueService, BookingQueueService>();

            this.serviceProvider = services.BuildServiceProvider();
            this.scope = this.serviceProvider.CreateScope();

            this.eventService = this.scope.ServiceProvider.GetRequiredService<IEventService>();
            this.bookingService = this.scope.ServiceProvider.GetRequiredService<IBookingService>();
        }

        [Fact]
        public async Task Booking_Add_Pending()
        {
            var eventItem = new EventRequest() { Title = "Test Add Pending", Description = "Описание 1", TotalSeats = 1, StartAt = DateTime.UtcNow.AddDays(-1), EndAt = DateTime.UtcNow.AddDays(2) };
            var eventId = await this.eventService.Add(eventItem, CancellationToken.None);
            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testloginadd", Role = "User" };
            
            var booking = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);


            Assert.True(booking?.Status == BookingStatus.Pending);
        }

        [Fact]
        public async Task Booking_Add_Id()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add Id", Description = "Описание 1", TotalSeats = 2, StartAt = DateTime.UtcNow.AddDays(-1), EndAt = DateTime.UtcNow.AddDays(2) }, CancellationToken.None);
            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testloginaddid", Role = "User" };

            var booking1 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            var booking2 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);


            Assert.True(booking1?.Id != booking2?.Id);
        }

        [Fact]
        public async Task Booking_Llmit_Exception()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add Id", Description = "Описание 1", TotalSeats = 15, StartAt = DateTime.UtcNow.AddDays(-1), EndAt = DateTime.UtcNow.AddDays(2) }, CancellationToken.None);
            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testloginlimit", Role = "User" };

            var booking1 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            var booking2 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            var booking3 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            var booking4 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            var booking5 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            var booking6 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            var booking7 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            var booking8 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            var booking9 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            var booking10 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);

            var exception = await Assert.ThrowsAsync<NoAvailableSeatsException>(() => this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None));
            Assert.Equal($"Превышен лимит бронированй для пользователя", exception.Message);
        }

        [Fact]
        public async Task Booking_Starded_Exception()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add Id", Description = "Описание 1", TotalSeats = 15, StartAt = DateTime.UtcNow.AddMinutes(15), EndAt = DateTime.UtcNow.AddDays(2) }, CancellationToken.None);
            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testloginlimit", Role = "User" };

            var exception = await Assert.ThrowsAsync<ValidationException>(() => this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None));
            Assert.Equal($"Событие уже началось", exception.Message);
        }

        [Fact]
        public async Task Booking_CanceledNotMine_Exception()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add Id", Description = "Описание 1", TotalSeats = 15, StartAt = DateTime.UtcNow.AddDays(-1), EndAt = DateTime.UtcNow.AddDays(2) }, CancellationToken.None);
            var user1 = new UserContext() { Id = Guid.NewGuid(), Login = "testloginmy", Role = "User" };
            var user2 = new UserContext() { Id = Guid.NewGuid(), Login = "testloginmy", Role = "User" };

           var bookingResult = await this.bookingService.CreateBookingAsync(eventId, user1, CancellationToken.None);

            var exception = await Assert.ThrowsAsync<ForbiddenExeption>(() => this.bookingService.Cancel(bookingResult.Id, user2, CancellationToken.None));
            Assert.Equal($"Не достаточно прав", exception.Message);
        }

        [Fact]
        public async Task Booking_CanceledAdmin_Exception()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add Id", Description = "Описание 1", TotalSeats = 15, StartAt = DateTime.UtcNow.AddDays(-1), EndAt = DateTime.UtcNow.AddDays(2) }, CancellationToken.None);
            var user1 = new UserContext() { Id = Guid.NewGuid(), Login = "testloginmy", Role = "User" };
            var user2 = new UserContext() { Id = Guid.NewGuid(), Login = "testloginmy", Role = "Admin" };

            var bookingResult = await this.bookingService.CreateBookingAsync(eventId, user1, CancellationToken.None);

            await this.bookingService.Cancel(bookingResult.Id, user2, CancellationToken.None);


            var booking = await this.bookingService.GetBookingByIdAsync(bookingResult.Id, user1, CancellationToken.None);

            Assert.True(booking.Status == BookingStatus.Cancelled);
        }


        [Fact]
        public async Task Bookink_GetById_Model()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test GetById", Description = "Описание 1", TotalSeats = 1, StartAt = DateTime.UtcNow.AddDays(-1), EndAt = DateTime.UtcNow.AddDays(2) }, CancellationToken.None);

            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testloginbyid", Role = "User" };

            var result = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            Booking? booking = null;
            if(result != null)
            {
                booking = await this.bookingService.GetBookingByIdAsync(result.Id, user, CancellationToken.None);
            }

            Assert.NotNull(booking);

        }
        [Fact]
        public async Task Booking_Add_NoAvailableSeats()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add NoAvailableSeats", Description = "Описание 1", TotalSeats = 2, StartAt = DateTime.UtcNow.AddDays(-1), EndAt = DateTime.UtcNow.AddDays(2) }, CancellationToken.None);
            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testloginseats", Role = "User" };

            var booking1 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            var booking2 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);

            var exception = await Assert.ThrowsAsync<NoAvailableSeatsException>(() => this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None));

            Assert.Equal($"Свободных мест на это мероприятие нет.", exception.Message);
        }

        [Fact]
        public async Task Booking_AddNoEvemtId_Throw()
        {
            var id = Guid.NewGuid();
            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testloginnoevemt", Role = "User" };
            var exception = await Assert.ThrowsAsync<EventException>(() => this.bookingService.CreateBookingAsync(id, user, CancellationToken.None));


            Assert.Equal($"Событие с идентификатором {id} не найден", exception.Message);
        }

        [Fact]
        public async Task Booking_AddNoId_Throw()
        {
            var id = Guid.NewGuid();
            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testloginnoid", Role = "User" };
            var exception = await Assert.ThrowsAsync<EventException>(() => this.bookingService.GetBookingByIdAsync(id, user, CancellationToken.None));


            Assert.Equal($"Бронирование с идентификатором {id} не найден", exception.Message);
        }

        [Fact]
        public async Task Booking_Add_Confirm()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add NoAvailableSeats", Description = "Описание 1", TotalSeats = 2, StartAt = DateTime.UtcNow.AddDays(-1), EndAt = DateTime.UtcNow.AddDays(2) }, CancellationToken.None);
            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testloginconfirm", Role = "User" };

            var result = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            var booking = await this.bookingService.GetBookingByIdAsync(result.Id, user, CancellationToken.None);
            booking.Confirm();




            Assert.True(booking.Status == BookingStatus.Confirmed);
            Assert.True(booking.ProcessedAt != null);
        }

        [Fact]
        public async Task Booking_Add_Reject()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add NoAvailableSeats", Description = "Описание 1", TotalSeats = 1, StartAt = DateTime.UtcNow.AddDays(-1), EndAt = DateTime.UtcNow.AddDays(2) }, CancellationToken.None);
            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testlogin", Role = "User" };

            var result = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            Booking? booking = null;
            if (result != null)
            {
                booking = await this.bookingService.GetBookingByIdAsync(result.Id, user, CancellationToken.None);
                booking?.Reject();
            }


            await this.eventService.ReleaseSeats(eventId);



            var result2 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);

            Assert.True(booking?.Status == BookingStatus.Rejected);
            Assert.True(booking.ProcessedAt != null);
        }

        [Fact]
        public async Task Booking_Add_Сompetition()
        {
            var eventId = await this.eventService.Add(new EventRequest() { Title = "Test Add NoAvailableSeats", Description = "Описание 1", TotalSeats = 5, StartAt = DateTime.UtcNow.AddDays(-1), EndAt = DateTime.UtcNow.AddDays(2) }, CancellationToken.None);
            var eventItem = await this.eventService.GetAsync(eventId, CancellationToken.None);

            var tasks = new Task<bool>[20];         

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {

                     return eventItem.TryReserveSeats();
                });
            }

            var taskResults = await Task.WhenAll(tasks);

            Assert.True(5 == taskResults.Where(s => s == true).Count());
            Assert.True(15 == taskResults.Where(s => s == false).Count());
            Assert.True(eventItem.AvailableSeats == 0);
        }
    }

}
