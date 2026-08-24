using Events.Domain.Entities;
using Events.Infrastructure.DataAccess;
using Events.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Events.Integration.Test;

[Collection("Database")]
public class InboxRepositoryTest : DataContainer
{
    [Fact]
    public async Task Inbox_Add_ExeptionUni()
    {
        try
        {
            await ResetDatabaseAsync();
            await using var context = CreateContext();
            var inboxRepository = new InboxRepository(context);
            var id = Guid.NewGuid();

            using (var context1 = new EventDbContext(options))
            {
                var firstMessage = new InboxMessage(id, DateTime.UtcNow);
                context1.InboxMessages.Add(firstMessage);
                await context1.SaveChangesAsync();
            } //

            using (var context2 = new EventDbContext(options))
            {
                var duplicateMessage = new InboxMessage(id, DateTime.UtcNow);
                context2.InboxMessages.Add(duplicateMessage);
                await context2.SaveChangesAsync();
            }

        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            Assert.True(true);
        }
        catch (Exception ex)
        {
            Assert.True(false);
        }
    }
}
