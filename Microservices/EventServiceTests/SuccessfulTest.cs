using Event.Domain.Interfaces;
using Event.Domain.Models;
using Event.Domain.Services;
using EventDomain.Extentions;
using static System.Net.WebRequestMethods;

namespace EventServiceTests
{
    public class SuccessfulTest
    {
        private readonly IEventService service;

        public SuccessfulTest()
        {
            this.service = new EventService();
            this.service.Add(new EventRequest() { Title = "Test 1", Description = "Описание 1", StartAt = new DateTime(2025, 05,11), EndAt = new DateTime(2025, 05, 12) });
            this.service.Add(new EventRequest() { Title = "Test 2", Description = "Описание 2", StartAt = new DateTime(2025, 05, 12), EndAt = new DateTime(2025, 05, 13) });
            this.service.Add(new EventRequest() { Title = "Test 3", Description = "Описание 3", StartAt = new DateTime(2025, 05, 13), EndAt = new DateTime(2025, 05, 14) });
            this.service.Add(new EventRequest() { Title = "Test 4", Description = "Описание 4", StartAt = new DateTime(2025, 05, 14), EndAt = new DateTime(2025, 05, 15) });
            this.service.Add(new EventRequest() { Title = "Test 5", Description = "Описание 5", StartAt = new DateTime(2025, 05, 15), EndAt = new DateTime(2025, 05, 16) });
            this.service.Add(new EventRequest() { Title = "Test 6", Description = "Описание 6", StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) });
        }

        [Fact]
        public void Event_GetAll_list()
        {
            var list = this.service.Get();

            Assert.True((list.Items.Count() == 6));
        }

        [Fact]
        public void Event_GetById_model()
        {
            var request = new EventRequest() { Title = "AddTestById", Description = "Тестовое добаление", StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) };
            var id = this.service.Add(request);

            var item = this.service.Get(id);

            Assert.NotNull(item);
        }


        [Fact]
        public void Event_Created_id()
        {
            var request = new EventRequest() { Title = "AddTest", Description = "Тестовое добаление", StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) };
            var id = this.service.Add(request);

            var item = this.service.Get(id);

            Assert.Equal(id, item.Id);
        }

        [Fact]
        public void Event_Update_true()
        {
            var requestAdd = new EventRequest() { Title = "AddTest", Description = "Тестовое добаление", StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) };
            var id = this.service.Add(requestAdd);

            var requestEdit = new EventRequest() { Title = "UpdateTest", Description = "Тестовое изменение", StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) };
            this.service.Update(id, requestEdit);

            var item = this.service.Get(id);

            Assert.Equal(requestEdit.Title, item.Title);
        }


        [Fact]
        public void Event_Delete_exception()
        {
            var requestAdd = new EventRequest() { Title = "AddTest", Description = "Тестовое добаление для удаления", StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) };
            var id = this.service.Add(requestAdd);


            this.service.Delete(id);


            Assert.Throws<EventException>(() => this.service.Get(id));
        }

        [Fact]
        public void Event_TitileFilter_Item()
        {
            string titleSearch = "Test";
            var expectedResult = new List<string> { "Test 2", "Test 3", "Test 4", "Test 5", "Test 6" };


            var list = this.service.Get();
            var filter = list.Items.Filter(titleSearch, null, null).ToList();

            Assert.All(filter, events => expectedResult.Contains(events.Title));
        }

        [Fact]
        public void Event_DatesFilter_Item()
        {

            var expectedResult = new List<DateTime> { new DateTime(2025, 05, 13), new DateTime(2025, 05, 14), new DateTime(2025, 05, 15), new DateTime(2025, 05, 16) };


            var list = this.service.Get();
            var filter = list.Items.Filter(null, new DateTime(2025, 05, 13), new DateTime(2025, 05, 16)).ToList();

            Assert.All(filter, events => expectedResult.Any(s => s == events.StartAt || s == events.EndAt));
        }

        [Fact]
        public void Event_Pagination_Item()
        {

            int page = 2;
            int pageSize = 4;


            var result = this.service.Get(null, null, null, page, pageSize);
   
            Assert.True(result.TotalPages == 2);
            Assert.True(result.CurrentPage == 2);
        }

    }
}
