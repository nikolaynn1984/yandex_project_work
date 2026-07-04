using EventDomain.DataAccess;
using EventDomain.Extentions;
using EventDomain.Interfaces;
using EventDomain.Models;
using EventDomain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace EventServiceTests
{
    public class SuccessfulTest
    {
        private readonly ServiceProvider serviceProvider;
        private readonly IServiceScope scope;
        private readonly IEventService service;
        private readonly IBookingService bookingService;

        public SuccessfulTest()
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

            this.service = this.scope.ServiceProvider.GetRequiredService<IEventService>();


            //Task[] tasks =
            //[
            //    this.service.Add(new EventRequest() { Title = "Test 1", Description = "Описание 1",TotalSeats = 1, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) }),
            //    this.service.Add(new EventRequest() { Title = "Test 2", Description = "Описание 2",TotalSeats = 1, StartAt = new DateTime(2025, 05, 12), EndAt = new DateTime(2025, 05, 13) }),
            //    this.service.Add(new EventRequest() { Title = "Test 3", Description = "Описание 3",TotalSeats = 1, StartAt = new DateTime(2025, 05, 13), EndAt = new DateTime(2025, 05, 14) }),
            //    this.service.Add(new EventRequest() { Title = "Test 4", Description = "Описание 4",TotalSeats = 1, StartAt = new DateTime(2025, 05, 14), EndAt = new DateTime(2025, 05, 15) }),
            //    this.service.Add(new EventRequest() { Title = "Test 5", Description = "Описание 5",TotalSeats = 1, StartAt = new DateTime(2025, 05, 15), EndAt = new DateTime(2025, 05, 16) }),
            //    this.service.Add(new EventRequest() { Title = "Test 6", Description = "Описание 6",TotalSeats = 1, StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) }),
            //];
            //Task.WaitAll(tasks);
        }

        [Fact]
        public async Task Event_GetAll_list()
        {
            var serviceAll = this.scope.ServiceProvider.GetRequiredService<IEventService>();

            await serviceAll.Add(new EventRequest() { Title = "Test 1", Description = "Описание 1", TotalSeats = 1, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) });
            await serviceAll.Add(new EventRequest() { Title = "Test 2", Description = "Описание 2", TotalSeats = 1, StartAt = new DateTime(2025, 05, 12), EndAt = new DateTime(2025, 05, 13) });
            await serviceAll.Add(new EventRequest() { Title = "Test 3", Description = "Описание 3", TotalSeats = 1, StartAt = new DateTime(2025, 05, 13), EndAt = new DateTime(2025, 05, 14) });
            await serviceAll.Add(new EventRequest() { Title = "Test 4", Description = "Описание 4", TotalSeats = 1, StartAt = new DateTime(2025, 05, 14), EndAt = new DateTime(2025, 05, 15) });
            await serviceAll.Add(new EventRequest() { Title = "Test 5", Description = "Описание 5", TotalSeats = 1, StartAt = new DateTime(2025, 05, 15), EndAt = new DateTime(2025, 05, 16) });
            await serviceAll.Add(new EventRequest() { Title = "Test 6", Description = "Описание 6", TotalSeats = 1, StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) });


            var list = await serviceAll.Get();

            Assert.True((list.Items.Count() == 6));
        }

        [Fact]
        public async Task Event_GetById_model()
        {
            var request = new EventRequest() { Title = "AddTestById", Description = "Тестовое добаление", TotalSeats = 1, StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) };
            var id = await this.service.Add(request);

            var item = await this.service.GetAsync(id);

            Assert.NotNull(item);
        }


        [Fact]
        public async Task Event_Created_id()
        {
            var request = new EventRequest() { Title = "AddTest", Description = "Тестовое добаление", TotalSeats = 1, StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) };
            var id = await this.service.Add(request);

            var item = await this.service.GetAsync(id);

            Assert.Equal(id, item.Id);
        }

        [Fact]
        public async Task Event_Update_true()
        {
            try
            {
                var requestAdd = new EventRequest() { Title = "AddTestUpdate", Description = "Тестовое добаление для обновления", TotalSeats = 1, StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) };
                var id = await this.service.Add(requestAdd, CancellationToken.None);

                var requestEdit = new EventRequest() { Title = "UpdateTest", Description = "Тестовое изменение", TotalSeats = 1, StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) };
                await this.service.Update(id, requestEdit, CancellationToken.None);

                var item = await this.service.GetAsync(id);

                Assert.Equal(requestEdit.Title, item.Title);

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Assert.True(false);
            }
            
        }


        [Fact]
        public async Task Event_Delete_exception()
        {
            var requestAdd = new EventRequest() { Title = "AddTest", Description = "Тестовое добаление для удаления", TotalSeats = 1, StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) };
            var id = await this.service.Add(requestAdd);


            await this.service.Delete(id);


            var exception = await Assert.ThrowsAsync<EventException>(() =>  this.service.GetAsync(id));

            string message = $"Событие с идентификатором {id} не найден";
            Assert.True(!string.IsNullOrEmpty(exception.Message));
        }

        [Fact]
        public async Task Event_TitileFilter_Item()
        {
            string titleSearch = "Test";
            var expectedResult = new List<string> { "Test 2", "Test 3", "Test 4", "Test 5", "Test 6" };


            var list = await this.service.Get();
            var filter = list.Items.Filter(titleSearch, null, null).ToList();

            Assert.All(filter, events => expectedResult.Contains(events.Title));
        }

        [Fact]
        public async Task Event_DatesFilter_Item()
        {

            var expectedResult = new List<DateTime> { new DateTime(2025, 05, 13), new DateTime(2025, 05, 14), new DateTime(2025, 05, 15), new DateTime(2025, 05, 16) };


            var list = await this.service.Get();
            var filter = list.Items.Filter(null, new DateTime(2025, 05, 13), new DateTime(2025, 05, 16)).ToList();

            Assert.All(filter, events => expectedResult.Any(s => s == events.StartAt || s == events.EndAt));
        }

        [Fact]
        public async Task Event_Pagination_Item()
        {

            int page = 2;
            int pageSize = 4;

            await this.service.Add(new EventRequest() { Title = "Test 1 1", Description = "Описание 1", TotalSeats = 1, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) });
            await this.service.Add(new EventRequest() { Title = "Test 1 2", Description = "Описание 2", TotalSeats = 1, StartAt = new DateTime(2025, 05, 12), EndAt = new DateTime(2025, 05, 13) });
            await this.service.Add(new EventRequest() { Title = "Test 3 3", Description = "Описание 3", TotalSeats = 1, StartAt = new DateTime(2025, 05, 13), EndAt = new DateTime(2025, 05, 14) });
            await this.service.Add(new EventRequest() { Title = "Test 4 4", Description = "Описание 4", TotalSeats = 1, StartAt = new DateTime(2025, 05, 14), EndAt = new DateTime(2025, 05, 15) });
            await this.service.Add(new EventRequest() { Title = "Test 5 5", Description = "Описание 5", TotalSeats = 1, StartAt = new DateTime(2025, 05, 15), EndAt = new DateTime(2025, 05, 16) });
            await this.service.Add(new EventRequest() { Title = "Test 6 6", Description = "Описание 6", TotalSeats = 1, StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) });

            var result = await this.service.Get(null, null, null, page, pageSize);
   
            Assert.True(result.TotalPages == 2);
            Assert.True(result.CurrentPage == 2);
        }


        [Fact]
        public async Task CreateBookingAsync_ConcurrentRequests_DoesNotOverbookEvent()
        {
            const int totalSeats = 5;
            const int concurrentRequests = 20;
            var eventId = await CreateTestEventAsync(totalSeats: totalSeats);

            var @event = await this.service.GetAsync(eventId);

            var tasks = Enumerable.Range(0, concurrentRequests)
                .Select(_ => Task.Run(async () =>
                {
                    using var scope = this.serviceProvider.CreateScope();
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    try
                    {
                        await bookingService.CreateBookingAsync(eventId);
                        return true;
                    }
                    catch (NoAvailableSeatsException)
                    {
                        return false;
                    }
                }));

            var results = await Task.WhenAll(tasks);

            var successCount = results.Count(r => r);
            Assert.Equal(totalSeats, successCount);
        }


        [Fact]
        public async Task CreateBookingAsync_ConcurrentRequests_AllSuccessfulBookingsHaveUniqueIds()
        {
            const int totalSeats = 10;
            const int concurrentRequests = 10;
            var eventId = await CreateTestEventAsync(totalSeats: totalSeats);
            var bookingIds = new System.Collections.Concurrent.ConcurrentBag<Guid>();

            var tasks = Enumerable.Range(0, concurrentRequests)
                .Select(_ => Task.Run(async () =>
                {
                    using var scope = this.serviceProvider.CreateScope();
                    var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                    var booking = await bookingService.CreateBookingAsync(eventId);
                    bookingIds.Add(booking.Id);
                }));

            await Task.WhenAll(tasks);

            Assert.Equal(totalSeats, bookingIds.Distinct().Count());
        }

        private async Task<Guid> CreateTestEventAsync(int totalSeats = 10)
        {
            var futureDate = DateTime.UtcNow.AddDays(1);
            var created = await this.service.Add(new EventRequest
            {
                Title = "Test Event",
                StartAt = futureDate,
                EndAt = futureDate.AddHours(2),
                TotalSeats = totalSeats
            });
            return created;
        }
    }
}
