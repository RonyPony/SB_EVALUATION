using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.BACKEND.Application.Common;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Domain.Common;
using SB.BACKEND.Domain.GovernmentEntities;
using SB.BACKEND.Domain.Security;
using SB.BACKEND.Domain.Support;

namespace SB.BACKEND.Infrastructure.Persistence;

public sealed class SecurityDbContext(
    DbContextOptions<SecurityDbContext> options,
    ICurrentUserService? currentUser = null
) : DbContext(options), IUnitOfWork
{
    private readonly ICurrentUserService? _currentUser = currentUser;

    public DbSet<User> Users
    {
        get { return Set<User>(); }
    }

    public DbSet<Role> Roles
    {
        get { return Set<Role>(); }
    }

    public DbSet<Permission> Permissions
    {
        get { return Set<Permission>(); }
    }

    public DbSet<UserRole> UserRoles
    {
        get { return Set<UserRole>(); }
    }

    public DbSet<RolePermission> RolePermissions
    {
        get { return Set<RolePermission>(); }
    }

    public DbSet<EntidadGubernamental> EntidadesGubernamentales
    {
        get { return Set<EntidadGubernamental>(); }
    }

    public DbSet<Area> Areas
    {
        get { return Set<Area>(); }
    }

    public DbSet<SolicitudSoporte> SolicitudesSoporte
    {
        get { return Set<SolicitudSoporte>(); }
    }

    public DbSet<HistorialSolicitud> HistorialSolicitudes
    {
        get { return Set<HistorialSolicitud>(); }
    }

    public DbSet<ComentarioSolicitud> ComentariosSolicitud
    {
        get { return Set<ComentarioSolicitud>(); }
    }

    public DbSet<Notificacion> Notificaciones
    {
        get { return Set<Notificacion>(); }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SecurityDbContext).Assembly);
        modelBuilder.HasSequence<long>("SolicitudCodeSequence").StartsAt(1).IncrementsBy(1);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.ApplyCreatedAudit(now, _currentUser?.UserId);
            else if (entry.State == EntityState.Modified)
                entry.Entity.ApplyUpdatedAudit(now, _currentUser?.UserId);
        }
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new ConflictException("A record with the same unique value already exists.");
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictException(
                "El registro fue modificado por otro usuario. Recargue los datos."
            );
        }
    }
}
