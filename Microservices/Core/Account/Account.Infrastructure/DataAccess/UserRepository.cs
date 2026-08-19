using Account.Application.Abstractions.Repositories;
using Account.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventInfrastructure.DataAccess.Account;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext context;

    public UserRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<User?> GetByLogin(string login, CancellationToken cancellationToken = default)
    {
        return await this.context.Users.FirstOrDefaultAsync(s => s.Login == login, cancellationToken);
    }

    public async Task<User?> Login(string login, string passwordHas, CancellationToken cancellationToken = default)
    {
        return await this.context.Users.FirstOrDefaultAsync(s => s.Login == login && s.PasswordHash == passwordHas, cancellationToken);
    }

    public async Task Register(User user, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(user, cancellationToken);

        await context.SaveChangesAsync();
    }
}
