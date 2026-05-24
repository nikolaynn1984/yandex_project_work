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
        public async Task<AddBookingResult> CreateBookingAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var tsc = new TaskCompletionSource<AddBookingResult>();

            _ = Task.Run(() =>
            {
                try
                {
                    this.eventService.Get(eventId);

                    cancellationToken.ThrowIfCancellationRequested();
                        

                    var booking = Add(eventId);


                    tsc.TrySetResult(new AddBookingResult(booking.Id, eventId, booking.Status));

                }
                catch(EventException exe)
                {
                    tsc.TrySetException(exe);
                }
            });

           return await tsc.Task;
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
            var tsc = new TaskCompletionSource<Booking>();
            _ = Task.Run(() =>
            {

                cancellationToken.ThrowIfCancellationRequested();


                var book = this.bookings.FirstOrDefault(s => s.Id == bookingId);
                if (book == null)
                {
                    tsc.TrySetException(new EventException($"Бронирование с идентификатором {bookingId} не найден"));
                }


#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
                tsc.TrySetResult(book);
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            });


            return await tsc.Task;
        }
    }
}
