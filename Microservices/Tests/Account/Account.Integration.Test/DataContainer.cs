using Account.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Account.Integration.Test
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

        protected UserDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseNpgsql(container.GetConnectionString(), s => s.MigrationsAssembly("Account.Server"))
                .Options;

            var context = new UserDbContext(options);
            context.Database.Migrate();
            return context;
        }

        protected async Task ResetDatabaseAsync()
        {
            await using var context = CreateContext();
            await context.Database.ExecuteSqlRawAsync(
                "TRUNCATE TABLE users RESTART IDENTITY CASCADE");
        }
    }
}
