using Events.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Events.Integration.Test
{
    public class DataContainer : IAsyncLifetime
    {
        protected readonly PostgreSqlContainer container = new PostgreSqlBuilder("postgres:16-alpine")
        .Build();

        internal DbContextOptions<EventDbContext> options;

        public async Task DisposeAsync()
        {
            await container.DisposeAsync();
        }

        public async Task InitializeAsync()
        {
            await container.StartAsync();
        }

        protected EventDbContext CreateContext()
        {
            options = new DbContextOptionsBuilder<EventDbContext>()
                .UseNpgsql(container.GetConnectionString(), s => s.MigrationsAssembly("Events.Server"))
                .Options;

            var context = new EventDbContext(options);
            context.Database.Migrate();
            return context;
        }

        protected async Task ResetDatabaseAsync()
        {
            await using var context = CreateContext();
            await context.Database.ExecuteSqlRawAsync(
                "TRUNCATE TABLE events RESTART IDENTITY CASCADE");
        }
    }
}
