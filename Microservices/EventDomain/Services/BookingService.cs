using Event.Domain.Interfaces;
using EventDomain.Extentions;
using EventDomain.Interfaces;
using EventDomain.Models;
using System.Collections.Concurrent;

namespace EventDomain.Services
{
    /// <summary>
    /// Сервис бронирования билетов
    /// </summary>
    public class BookingService : IBookingService
    {
        private readonly ConcurrentBag<Booking> bookings;
        private readonly IEventService eventService;
        private readonly IBookingQueueService bookingQueueService;

        public BookingService(IEventService eventService, IBookingQueueService bookingQueueService)
        {
            this.eventService = eventService;
            this.bookings = new ConcurrentBag<Booking>();
            this.bookingQueueService = bookingQueueService;
        }

        /// <inheritdoc/>
        public async Task<AddBookingResult?> CreateBookingAsync(Guid eventId, CancellationToken cancellationToken = default)
        {


             await this.eventService.Get(eventId);

             if (cancellationToken.IsCancellationRequested)
                 return null;


             var booking = Add(eventId);


             return new AddBookingResult(booking.Id, eventId, booking.Status);
        }

        /// <summary>
        /// Добавление планирования
        /// </summary>
        /// <param name="eventId">Идентификатор события</param>
        /// <returns>Объектная модель бронирования</returns>
        private Booking Add(Guid eventId)
        {

            var id = Guid.NewGuid();

            var booking = new Booking(id, eventId);

            this.bookings.Add(booking);

            this.bookingQueueService.Enqueue(booking);

            return booking;
        }

        /// <inheritdoc/>
        public async Task<Booking> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<Booking>();
            _ = Task.Run(() =>
            {

                if (cancellationToken.IsCancellationRequested)
                    tcs.TrySetCanceled();


                var book = this.bookings.FirstOrDefault(s => s.Id == bookingId);
                if (book == null)
                {
                    tcs.TrySetException(new EventException($"Бронирование с идентификатором {bookingId} не найден"));
                }


#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
                tcs.TrySetResult(book);
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            }, cancellationToken);


            return await tcs.Task;
        }
    }
}
