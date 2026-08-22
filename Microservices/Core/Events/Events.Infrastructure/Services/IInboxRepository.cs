using Events.Application.Abstractions.Repositories;
using Events.Domain.Entities;
using Events.Infrastructure.DataAccess;

namespace Events.Infrastructure.Services;

internal class InboxRepository : IInboxRepository
{
    private readonly EventDbContext context;

    internal InboxRepository(EventDbContext context)
    {
        this.context = context;
    }


    public async Task Add(InboxMessage message, CancellationToken cancellationToken = default)
    {
        await this.context.InboxMessages.AddAsync(message, cancellationToken);
    }
}
