using EventApplication.Abstractions.Repositories;
using EventDomain;
using EventDomain.Entities;
using EventDomain.Exceptions;
using EventInfrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            throw new EventException($"Бронирования с идентификатором события {EventId} не найдены");
        }

        return books;
    }

    public async Task<List<Booking>> GetByUser(Guid UserId, CancellationToken cancellationToken = default)
    {
        var books = await this.context.Bookings.Where(s => s.UserId == UserId && s.Status != BookingStatus.Cancelled).ToListAsync(cancellationToken);
        if (books == null)
        {
            throw new EventException($"Бронирования с идентификатором пользователя {UserId} не найдены");
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
