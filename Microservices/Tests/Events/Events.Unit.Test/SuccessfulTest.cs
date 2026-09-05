using Events.Application;
using Events.Application.Abstractions.Repositories;
using Events.Application.Abstractions.Services;
using Events.Application.Events.DTOs;
using Events.Domain.Exceptions;
using Events.Infrastructure.DataAccess;
using Events.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Events.Unit.Test
{
    public class SuccessfulTest
    {
        private readonly ServiceProvider serviceProvider;
        private readonly IServiceScope scope;
        private readonly IEventService service;
        private readonly DateTime StartAt = DateTime.UtcNow.AddDays(1);
        private readonly DateTime EndAt = DateTime.UtcNow.AddDays(3);

        public SuccessfulTest()
        {
            var dbName = Guid.NewGuid().ToString();
            var services = new ServiceCollection();
            services.AddDbContext<EventDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventService, EventService>();

            this.serviceProvider = services.BuildServiceProvider();
            this.scope = this.serviceProvider.CreateScope();

            this.service = this.scope.ServiceProvider.GetRequiredService<IEventService>();

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

            var item = await this.service.Get(id);

            Assert.NotNull(item);
        }


        [Fact]
        public async Task Event_Created_id()
        {
            var request = new EventRequest() { Title = "AddTest", Description = "Тестовое добаление", TotalSeats = 1, StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) };
            var id = await this.service.Add(request);

            var item = await this.service.Get(id);

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

                var item = await this.service.Get(id);

                Assert.Equal(requestEdit.Title, item.Title);

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Assert.True(false);
            }
            
        }

        [Fact]
        public async Task Event_Starded_NoValid()
        {
            var eventId = await this.service.Add(new EventRequest() { Title = "Test Add Id", Description = "Описание 1", TotalSeats = 15, StartAt = DateTime.UtcNow.AddMinutes(-15), EndAt = EndAt }, CancellationToken.None);

            var eventItem = await this.service.Get(eventId);

            bool valid = eventItem.TryValivadeStartAt();

            Assert.False(valid);
        }

        [Fact]
        public async Task Event_Release_NoAvailableSeats()
        {
            var eventId = await this.service.Add(new EventRequest() { Title = "Test Add NoAvailableSeats", Description = "Описание 1", TotalSeats = 2, StartAt = StartAt, EndAt = EndAt }, CancellationToken.None);
            var user = new UserContext() { Id = Guid.NewGuid(), Login = "testloginseats", Role = "User" };

            var eventItem = await this.service.Get(eventId);

            eventItem.TryReserveSeats(1);
            eventItem.TryReserveSeats(1);

            Assert.False(eventItem.TryReserveSeats(1));
        }


        [Fact]
        public async Task Event_Delete_exception()
        {
            var requestAdd = new EventRequest() { Title = "AddTest", Description = "Тестовое добаление для удаления", TotalSeats = 1, StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) };
            var id = await this.service.Add(requestAdd);


            await this.service.Delete(id);


            var exception = await Assert.ThrowsAsync<EventException>(() =>  this.service.Get(id));

            string message = $"Событие с идентификатором {id} не найден";
            Assert.True(!string.IsNullOrEmpty(exception.Message));
        }

        [Fact]
        public async Task Event_TitileFilter_Item()
        {
            string titleSearch = "Test";
            var expectedResult = new List<string> { "Test 2", "Test 3", "Test 4", "Test 5", "Test 6" };


            var list = await this.service.Get(titleSearch);

            Assert.All(list.Items, events => expectedResult.Contains(events.Title));
        }

        [Fact]
        public async Task Event_DatesFilter_Item()
        {

            var expectedResult = new List<DateTime> { new DateTime(2025, 05, 13), new DateTime(2025, 05, 14), new DateTime(2025, 05, 15), new DateTime(2025, 05, 16) };


            var list = await this.service.Get(null, new DateTime(2025, 05, 13), new DateTime(2025, 05, 16));

            Assert.All(list.Items, events => expectedResult.Any(s => s == events.StartAt || s == events.EndAt));
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
        public async Task Reserve_Seats_Сompetition()
        {
            var eventId = await this.service.Add(new EventRequest() { Title = "Test Add NoAvailableSeats", Description = "Описание 1", TotalSeats = 5, StartAt = StartAt, EndAt = EndAt }, CancellationToken.None);
            var eventItem = await this.service.Get(eventId, CancellationToken.None);

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
