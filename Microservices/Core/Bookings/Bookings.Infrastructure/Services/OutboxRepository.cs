using Bookings.Application.Abstractions.Repositories;
using Bookings.Domain.Entities;
using Bookings.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Infrastructure.Services;

public class OutboxRepository : IOutboxRepository
{
    private readonly BookingDbContext context;

    public OutboxRepository(BookingDbContext context)
    {
        this.context = context;
    }

    public async Task Add(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await this.context.AddAsync(message, cancellationToken);
    }

    public async Task<IReadOnlyList<OutboxMessage>> GetNoProcessed(CancellationToken cancellationToken = default)
    {
        return await this.context.OutboxMessages.Where(s => s.IsProcessed == false)
            .OrderBy(s => s.OccurredOn)
            .Take(20)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await this.context.SaveChangesAsync(cancellationToken);
    }
}
