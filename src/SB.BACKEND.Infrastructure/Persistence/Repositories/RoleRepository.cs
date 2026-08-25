using Microsoft.EntityFrameworkCore;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Domain.Security;

namespace SB.BACKEND.Infrastructure.Persistence.Repositories;

internal sealed class RoleRepository(SecurityDbContext dbContext) : IRoleRepository
{
    public async Task<IReadOnlyCollection<Role>> GetAllAsync(CancellationToken ct) =>
        await dbContext.Roles.AsNoTracking().Include(x => x.RolePermissions).ThenInclude(x => x.Permission).OrderBy(x => x.Name).ToListAsync(ct);
    public Task<Role?> GetByIdAsync(Guid id, CancellationToken ct) => dbContext.Roles.Include(x => x.RolePermissions).ThenInclude(x => x.Permission).SingleOrDefaultAsync(x => x.Id == id, ct);
    public Task<Role?> GetByNameAsync(string name, CancellationToken ct) => dbContext.Roles.Include(x => x.RolePermissions).ThenInclude(x => x.Permission).SingleOrDefaultAsync(x => x.NormalizedName == name, ct);
    public Task<bool> NameExistsAsync(string name, Guid? excludingId, CancellationToken ct) => dbContext.Roles.AnyAsync(x => x.NormalizedName == name && (!excludingId.HasValue || x.Id != excludingId), ct);
    public void Add(Role role) => dbContext.Roles.Add(role);
    public void Remove(Role role) => dbContext.Roles.Remove(role);
}
