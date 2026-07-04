using EventDomain.DataAccess;
using EventDomain.Extentions;
using EventDomain.Interfaces;
using EventDomain.Models;
using Microsoft.EntityFrameworkCore;

namespace EventDomain.Repository
{
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
}
