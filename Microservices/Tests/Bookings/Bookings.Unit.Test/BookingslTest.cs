using Bookings.Application;
using Bookings.Application.Abstractions.Repositories;
using Bookings.Application.Abstractions.Services;
using Bookings.Application.DTOs;
using Bookings.Domain;
using Bookings.Domain.Entities;
using Bookings.Domain.Exceptions;
using Bookings.Infrastructure.DataAccess;
using Bookings.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventServiceTests
{
    public class BookingslTest
    {
        private readonly ServiceProvider serviceProvider;
        private readonly IServiceScope scope;
        private readonly IBookingService bookingService;

        public BookingslTest()
        {

            var dbName = Guid.NewGuid().ToString();
            var services = new ServiceCollection();
            services.AddDbContext<BookingDbContext>(options =>
                options.UseInMemoryDatabase(dbName));
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBookingValidator, BookingValidator>();
            services.AddSingleton<IBookingQueueService, BookingQueueService>();

            this.serviceProvider = services.BuildServiceProvider();
            this.scope = this.serviceProvider.CreateScope();

            this.bookingService = this.scope.ServiceProvider.GetRequiredService<IBookingService>();
        }

        [Fact]
        public async Task Booking_Add_Pending()
        {
            var eventId = Guid.NewGuid();
            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testloginadd", Role = "User" };
            
            var booking = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);


            Assert.True(booking?.Status == BookingStatus.Pending);
        }

        [Fact]
        public async Task Booking_Add_Id()
        {
            var eventId = Guid.NewGuid();
            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testloginaddid", Role = "User" };

            var booking1 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            var booking2 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);


            Assert.True(booking1?.Id != booking2?.Id);
        }

        [Fact]
        public async Task Booking_Llmit_Exception()
        {
            var eventId = Guid.NewGuid();
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
            Assert.Equal($"Превышен лимит (10) бронированй для пользователя", exception.Message);
        }

        

        [Fact]
        public async Task Booking_CanceledNotMine_Exception()
        {
            var eventId = Guid.NewGuid();
            var user1 = new UserContext() { Id = Guid.NewGuid(), Login = "testloginmy", Role = "User" };
            var user2 = new UserContext() { Id = Guid.NewGuid(), Login = "testloginmy", Role = "User" };

           var bookingResult = await this.bookingService.CreateBookingAsync(eventId, user1, CancellationToken.None);

            var exception = await Assert.ThrowsAsync<ForbiddenExeption>(() => this.bookingService.Cancel(bookingResult.Id, user2, CancellationToken.None));
            Assert.Equal($"Не достаточно прав", exception.Message);
        }

        [Fact]
        public async Task Booking_CanceledAdmin_Exception()
        {
            var eventId = Guid.NewGuid();
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
            var eventId = Guid.NewGuid();
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
        public async Task Booking_AddNoId_Throw()
        {
            var id = Guid.NewGuid();
            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testloginnoid", Role = "User" };
            var exception = await Assert.ThrowsAsync<BookingException>(() => this.bookingService.GetBookingByIdAsync(id, user, CancellationToken.None));


            Assert.Equal($"Бронирование с идентификатором {id} не найден", exception.Message);
        }

        [Fact]
        public async Task Booking_Add_Confirm()
        {
            var eventId = Guid.NewGuid();
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
            var eventId = Guid.NewGuid();
            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testlogin", Role = "User" };

            var result = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);
            Booking? booking = null;
            if (result != null)
            {
                booking = await this.bookingService.GetBookingByIdAsync(result.Id, user, CancellationToken.None);
                booking?.Reject();
            }



            var result2 = await this.bookingService.CreateBookingAsync(eventId, user, CancellationToken.None);

            Assert.True(booking?.Status == BookingStatus.Rejected);
            Assert.True(booking.ProcessedAt != null);
        }

        [Fact]
        public async Task CreateBookingAsync_ConcurrentRequests_AllSuccessfulBookingsHaveUniqueIds()
        {
            const int totalSeats = 10;
            const int concurrentRequests = 10;
            var eventId = Guid.NewGuid();
            var bookingIds = new System.Collections.Concurrent.ConcurrentBag<Guid>();

            var tasks = Enumerable.Range(0, concurrentRequests)
                .Select(_ => Task.Run(async () =>
                {
                    using var scope = this.serviceProvider.CreateScope();
                    var user = new UserContext() { Id = Guid.NewGuid(), Login = "testloginadd", Role = "User" };
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    var booking = await bookingService.CreateBookingAsync(eventId, user);
                    if (booking != null)
                        bookingIds.Add(booking.Id);
                }));

            await Task.WhenAll(tasks);

            Assert.Equal(totalSeats, bookingIds.Distinct().Count());
        }

    }

}
