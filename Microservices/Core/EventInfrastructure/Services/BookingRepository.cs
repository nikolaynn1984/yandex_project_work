using EventApplication.Abstractions.Repositories;
using EventDomain.Entities;
using EventDomain.Exceptions;
using EventInfrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace EventInfrastructure.Services;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext context;

    public BookingRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Booking> GetById(Guid Id, CancellationToken cancellationToken = default)
    {
        var book = await this.context.Bookings.FirstOrDefaultAsync(s => s.Id == Id, cancellationToken);
        if (book == null)
        {
            throw new EventException($"Бронирование с идентификатором {Id} не найден");
        }

        return book;
    }

    public async Task<List<Booking>> GetByEventId(Guid EventId, CancellationToken cancellationToken = default)
    {
        var books = await this.context.Bookings.Where(s => s.EventId == EventId).ToListAsync(cancellationToken);
        if (books == null)
        {
            throw new EventException($"Бронированиz с идентификатором собяти {EventId} не найден");
        }

        return books;
    }
    public async Task Add(Booking booking, CancellationToken cancellationToken = default)
    {
        await this.context.AddAsync(booking, cancellationToken);

        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await this.context.SaveChangesAsync(cancellationToken);
    }
}
