using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using SB.BACKEND.Application.Common;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Domain.Common;
using SB.BACKEND.Domain.GovernmentEntities;
using SB.BACKEND.Domain.Security;

namespace SB.BACKEND.Infrastructure.Persistence;

public sealed class SecurityDbContext : DbContext, IUnitOfWork
{
    private readonly ICurrentUserService? _currentUser;
    public SecurityDbContext(DbContextOptions<SecurityDbContext> options, ICurrentUserService? currentUser = null) : base(options) { _currentUser = currentUser; }
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<EntidadGubernamental> EntidadesGubernamentales => Set<EntidadGubernamental>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SecurityDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        { if (entry.State == EntityState.Added) entry.Entity.ApplyCreatedAudit(now, _currentUser?.UserId); else if (entry.State == EntityState.Modified) entry.Entity.ApplyUpdatedAudit(now, _currentUser?.UserId); }
        try { return await base.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        { throw new ConflictException("A record with the same unique value already exists."); }
        catch (DbUpdateConcurrencyException) { throw new ConflictException("El registro fue modificado por otro usuario. Recargue los datos."); }
    }
}
