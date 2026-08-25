using Microsoft.EntityFrameworkCore;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Domain.Security;

namespace SB.BACKEND.Infrastructure.Persistence.Repositories;

internal sealed class PermissionRepository(SecurityDbContext dbContext) : IPermissionRepository
{
    public async Task<IReadOnlyCollection<Permission>> GetAllAsync(CancellationToken ct) => await dbContext.Permissions.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
    public Task<Permission?> GetByIdAsync(Guid id, CancellationToken ct) => dbContext.Permissions.SingleOrDefaultAsync(x => x.Id == id, ct);
}
