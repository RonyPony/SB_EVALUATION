using Microsoft.EntityFrameworkCore;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Domain.Security;

namespace SB.BACKEND.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(SecurityDbContext dbContext) : IUserRepository
{
    public async Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken ct)
    {
        return await dbContext
            .Users.AsNoTracking()
            .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
            .OrderBy(x => x.Username)
            .ToListAsync(ct);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return dbContext
            .Users.Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                    .ThenInclude(x => x.RolePermissions)
                        .ThenInclude(x => x.Permission)
            .SingleOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<User?> GetByUsernameAsync(string normalizedUsername, CancellationToken ct)
    {
        return dbContext
            .Users.Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                    .ThenInclude(x => x.RolePermissions)
                        .ThenInclude(x => x.Permission)
            .SingleOrDefaultAsync(x => x.NormalizedUsername == normalizedUsername, ct);
    }

    public Task<bool> UsernameExistsAsync(string value, CancellationToken ct)
    {
        return dbContext.Users.AnyAsync(x => x.NormalizedUsername == value, ct);
    }

    public Task<bool> EmailExistsAsync(string value, CancellationToken ct)
    {
        return dbContext.Users.AnyAsync(x => x.NormalizedEmail == value, ct);
    }

    public void Add(User user)
    {
        dbContext.Users.Add(user);
    }
}
