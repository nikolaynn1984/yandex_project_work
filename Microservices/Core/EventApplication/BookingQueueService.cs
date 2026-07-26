using EventApplication.Abstractions.Services;
using EventDomain.Entities;
using System.Collections.Concurrent;

namespace EventApplication;

/// <summary>
/// Сервис обработки очереди бронирований
/// </summary>
public class BookingQueueService : IBookingQueueService
{
    private readonly ConcurrentQueue<Booking> bookingsQueue;
    public event Action<List<Booking>>? OnNextEvent;
    private readonly SemaphoreSlim _addRequestLock = new(1, 1);

    public BookingQueueService() => this.bookingsQueue = new ConcurrentQueue<Booking>();
    public void Add(Booking booking)
    {
        this.bookingsQueue.Enqueue(booking);

        _ = Task.Run(async () => await Next());
    }

    public async Task Next()
    {
        if (!await _addRequestLock.WaitAsync(1))
            return;

        await Task.Delay(1000);

        if (this.bookingsQueue.Count > 0)
        {
            sendEvent();
        }

        _addRequestLock.Release();
    }

    private void sendEvent()
    {
        List<Booking> bookings = new List<Booking>();

#pragma warning disable CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
        while (this.bookingsQueue.TryDequeue(out Booking booking))
        {
            bookings.Add(booking);
        }
#pragma warning restore CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.

        if (bookings.Count > 0)
            this.OnNextEvent?.Invoke(bookings);
    }
}
