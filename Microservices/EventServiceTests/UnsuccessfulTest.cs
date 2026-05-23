using Event.Domain.Interfaces;
using Event.Domain.Models;
using Event.Domain.Services;
using EventDomain.Extentions;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventServiceTests
{
    public class UnsuccessfulTest
    {
        private readonly IEventService service;

        public UnsuccessfulTest()
        {
            this.service = new EventService();
            this.service.Add(new EventRequest() { Title = "Test 1", Description = "Описание 1", StartAt = new DateTime(2025, 05, 11), EndAt = new DateTime(2025, 05, 12) });
            this.service.Add(new EventRequest() { Title = "Test 2", Description = "Описание 2", StartAt = new DateTime(2025, 05, 12), EndAt = new DateTime(2025, 05, 13) });
            this.service.Add(new EventRequest() { Title = "Test 3", Description = "Описание 3", StartAt = new DateTime(2025, 05, 13), EndAt = new DateTime(2025, 05, 14) });
            this.service.Add(new EventRequest() { Title = "Test 4", Description = "Описание 4", StartAt = new DateTime(2025, 05, 14), EndAt = new DateTime(2025, 05, 15) });
            this.service.Add(new EventRequest() { Title = "Test 5", Description = "Описание 5", StartAt = new DateTime(2025, 05, 15), EndAt = new DateTime(2025, 05, 16) });
            this.service.Add(new EventRequest() { Title = "Test 6", Description = "Описание 6", StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) });
        }

        [Fact]
        public void Event_GetById_Throw()
        {

            Assert.Throws<EventException>(() => this.service.Get(8));
        }

        [Fact]
        public void Event_Update_Throw()
        {
            var eventItem = new EventRequest() { Title = "Test 6", Description = "Описание 6", StartAt = new DateTime(2025, 05, 16), EndAt = new DateTime(2025, 05, 17) };
            int id = 9;

            Assert.Throws<EventException>(() => this.service.Update(id, eventItem));
        }
    }
}
