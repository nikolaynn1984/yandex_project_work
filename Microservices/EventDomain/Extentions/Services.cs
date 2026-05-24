using Event.Domain.Interfaces;
using Event.Domain.Services;
using EventDomain.Extentions;
using EventDomain.Interfaces;
using EventDomain.Services;
using Microsoft.AspNetCore.Builder;
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
            services.AddSingleton<IBookingService, BookingService>();
        }

        /// <summary>
        /// Промежуточное ПО обработки исключений
        /// </summary>
        /// <param name="builder">Строитель</param>
        /// <returns>Строитель</returns>
        public static void UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
}
