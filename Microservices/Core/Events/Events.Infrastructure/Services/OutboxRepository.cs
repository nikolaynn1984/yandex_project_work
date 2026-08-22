using Events.Application.Abstractions.Repositories;
using Events.Domain.Entities;
using Events.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure.Services;

internal class OutboxRepository : IOutboxRepository
{
    private readonly EventDbContext context;

    internal OutboxRepository(EventDbContext context)
    {
        this.context = context;
    }

    public async Task Add(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await this.context.OutboxMessages.AddAsync(message, cancellationToken);
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
