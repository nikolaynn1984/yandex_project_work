using Bookings.Application.Abstractions.Repositories;
using Bookings.Domain;
using Bookings.Domain.Entities;
using Bookings.Domain.Exceptions;
using Bookings.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Infrastructure.Services;

public class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext context;

    public BookingRepository(BookingDbContext context)
    {
        this.context = context;
    }

    public async Task<Booking> GetById(Guid Id, CancellationToken cancellationToken = default)
    {
        var book = await this.context.Bookings.FirstOrDefaultAsync(s => s.Id == Id, cancellationToken);
        if (book == null)
        {
            throw new BookingException($"Бронирование с идентификатором {Id} не найден");
        }

        return book;
    }

    public async Task<List<Booking>> GetByEventId(Guid EventId, CancellationToken cancellationToken = default)
    {
        var books = await this.context.Bookings.Where(s => s.EventId == EventId).ToListAsync(cancellationToken);
        if (books == null)
        {
            throw new BookingException($"Бронирования с идентификатором события {EventId} не найдены");
        }

        return books;
    }

    public async Task<List<Booking>> GetByUser(Guid UserId, CancellationToken cancellationToken = default)
    {
        var books = await this.context.Bookings.Where(s => s.UserId == UserId && s.Status != BookingStatus.Cancelled).ToListAsync(cancellationToken);
        if (books == null)
        {
            throw new BookingException($"Бронирования с идентификатором пользователя {UserId} не найдены");
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
