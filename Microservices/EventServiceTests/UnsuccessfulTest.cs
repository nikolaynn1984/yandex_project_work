using EventApplication;
using EventApplication.Abstractions.Repositories;
using EventApplication.Abstractions.Services;
using EventApplication.Events.DTOs;
using EventDomain.Exceptions;
using EventInfrastructure.DataAccess;
using EventInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventServiceTests
{
    public class UnsuccessfulTest
    {
        private readonly ServiceProvider serviceProvider;
        private readonly IServiceScope scope;
        private readonly IEventService service;

        public UnsuccessfulTest()
        {
            var dbName = Guid.NewGuid().ToString();
            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddSingleton<IBookingQueueService, BookingQueueService>();

            this.serviceProvider = services.BuildServiceProvider();
            this.scope = this.serviceProvider.CreateScope();

            this.service = this.scope.ServiceProvider.GetRequiredService<IEventService>();
            this.service.Add(new EventRequest() { Title = "Test 1", Description = "Описание 1", TotalSeats = 1, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) });
            this.service.Add(new EventRequest() { Title = "Test 2", Description = "Описание 2", TotalSeats = 1, StartAt = new DateTime(2025, 05, 12), EndAt = new DateTime(2025, 05, 13) });
            this.service.Add(new EventRequest() { Title = "Test 3", Description = "Описание 3", TotalSeats = 1, StartAt = new DateTime(2025, 05, 13), EndAt = new DateTime(2025, 05, 14) });
            this.service.Add(new EventRequest() { Title = "Test 4", Description = "Описание 4", TotalSeats = 1, StartAt = new DateTime(2025, 05, 14), EndAt = new DateTime(2025, 05, 15) });
            this.service.Add(new EventRequest() { Title = "Test 5", Description = "Описание 5", TotalSeats = 1, StartAt = new DateTime(2025, 05, 15), EndAt = new DateTime(2025, 05, 16) });
            this.service.Add(new EventRequest() { Title = "Test 6", Description = "Описание 6", TotalSeats = 1, StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) });
        }

       

        [Fact]
        public async Task Event_GetById_Throw()
        {
            var id = Guid.NewGuid();
            var exception = await Assert.ThrowsAsync<EventException>(() => this.service.GetAsync(id));

            Assert.Equal($"Событие с идентификатором {id} не найден", exception.Message);
        }

        [Fact]
        public async Task Event_Update_Throw()
        {
            var eventItem = new EventRequest() { Title = "Test 6", Description = "Описание 6", StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) };
            var id = Guid.NewGuid();

            var exception = await Assert.ThrowsAsync<EventException>(() => this.service.Update(id, eventItem));

            Assert.Equal($"Событие с идентификатором {id} не найден", exception.Message);
        }
    }
}
