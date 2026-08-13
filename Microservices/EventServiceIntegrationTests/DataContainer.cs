using EventInfrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace EventServiceIntegrationTests
{
    public class DataContainer : IAsyncLifetime
    {
        protected readonly PostgreSqlContainer container = new PostgreSqlBuilder("postgres:16-alpine")
        .Build();

        public async Task DisposeAsync()
        {
            await container.DisposeAsync();
        }

        public async Task InitializeAsync()
        {
            await container.StartAsync();
        }

        protected AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(container.GetConnectionString(), s => s.MigrationsAssembly("EventServer"))
                .Options;

            var context = new AppDbContext(options);
            context.Database.Migrate();
            return context;
        }

        protected async Task ResetDatabaseAsync()
        {
            await using var context = CreateContext();
            await context.Database.ExecuteSqlRawAsync(
                "TRUNCATE TABLE bookings, events RESTART IDENTITY CASCADE");
        }
    }
}
