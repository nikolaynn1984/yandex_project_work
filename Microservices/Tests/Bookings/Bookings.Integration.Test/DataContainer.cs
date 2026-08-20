using Bookings.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Bookings.Integration.Test
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

        protected BookingDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<BookingDbContext>()
                .UseNpgsql(container.GetConnectionString(), s => s.MigrationsAssembly("Bookings.Server"))
                .Options;

            var context = new BookingDbContext(options);
            context.Database.Migrate();
            return context;
        }

        protected async Task ResetDatabaseAsync()
        {
            await using var context = CreateContext();
            await context.Database.ExecuteSqlRawAsync(
                "TRUNCATE TABLE bookings RESTART IDENTITY CASCADE");
        }
    }
}
