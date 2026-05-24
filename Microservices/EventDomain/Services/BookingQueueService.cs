using EventDomain.Interfaces;
using EventDomain.Models;
using System.Collections.Concurrent;

namespace EventDomain.Services;

/// <summary>
/// Сервис обработки очереди бронирований
/// </summary>
public class BookingQueueService : IBookingQueueService
{
    private readonly ConcurrentQueue<Booking> bookingsQueue;

    public BookingQueueService()
    {
        this.bookingsQueue = new ConcurrentQueue<Booking>();
    }
    public void Enqueue(Booking booking)
    {
        this.bookingsQueue.Enqueue(booking);
    }

    public bool TryDequeue(out Booking booking)
    {
        return this.bookingsQueue.TryDequeue(out  booking);
    }
}
