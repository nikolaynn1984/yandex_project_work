using Event.Domain.Interfaces;
using Event.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Event.Domain.Extentions
{
    /// <summary>
    /// Сервисы
    /// </summary>
    public static class Services
    {
        /// <summary>
        /// Добавление сервсива обработки событий
        /// </summary>
        /// <param name="services">Коллекция сервисов</param>
        public static void AddEventService(this IServiceCollection services)
        {
            services.AddSingleton<IEventService, EventService>();
        }
    }
}
