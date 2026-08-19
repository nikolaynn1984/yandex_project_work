using Events.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Events.Infrastructure.DataAccess;

public class EventDbContext : DbContext
{
    public EventDbContext(DbContextOptions<EventDbContext> options) : base(options) { }

    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventDbContext).Assembly);
    }
}
