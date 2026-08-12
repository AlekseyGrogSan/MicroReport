using Microsoft.EntityFrameworkCore;
using UserService.Core.Models;
using UserService.Core.Interfaces;

namespace UserService.Data.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await dbContext.Users.Where(u => u.Id == id).ExecuteDeleteAsync(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
