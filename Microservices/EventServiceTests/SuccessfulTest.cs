using Event.Domain.Interfaces;
using Event.Domain.Models;
using Event.Domain.Services;
using EventDomain.Extentions;
using System.Diagnostics;

namespace EventServiceTests
{
    public class SuccessfulTest
    {
        private readonly IEventService service;

        public SuccessfulTest()
        {
            this.service = new EventService();

            Task[] tasks =
            [
                this.service.Add(new EventRequest() { Title = "Test 1", Description = "Описание 1",TotalSeats = 1, StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) }),
                this.service.Add(new EventRequest() { Title = "Test 2", Description = "Описание 2",TotalSeats = 1, StartAt = new DateTime(2025, 05, 12), EndAt = new DateTime(2025, 05, 13) }),
                this.service.Add(new EventRequest() { Title = "Test 3", Description = "Описание 3",TotalSeats = 1, StartAt = new DateTime(2025, 05, 13), EndAt = new DateTime(2025, 05, 14) }),
                this.service.Add(new EventRequest() { Title = "Test 4", Description = "Описание 4",TotalSeats = 1, StartAt = new DateTime(2025, 05, 14), EndAt = new DateTime(2025, 05, 15) }),
                this.service.Add(new EventRequest() { Title = "Test 5", Description = "Описание 5",TotalSeats = 1, StartAt = new DateTime(2025, 05, 15), EndAt = new DateTime(2025, 05, 16) }),
                this.service.Add(new EventRequest() { Title = "Test 6", Description = "Описание 6",TotalSeats = 1, StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) }),
            ];
            Task.WaitAll(tasks);
        }

        [Fact]
        public async Task Event_GetAll_list()
        {
            var serviceAll = new EventService();
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


            var result = await this.service.Get(null, null, null, page, pageSize);
   
            Assert.True(result.TotalPages == 2);
            Assert.True(result.CurrentPage == 2);
        }

    }
}
