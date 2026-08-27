using Microsoft.EntityFrameworkCore;
using SB.BACKEND.Application.GovernmentEntities;
using SB.BACKEND.Domain.GovernmentEntities;

namespace SB.BACKEND.Infrastructure.Persistence.Repositories;

internal sealed class GovernmentEntityRepository(SecurityDbContext db) : IGovernmentEntityRepository
{
    public async Task<(IReadOnlyCollection<EntidadGubernamental> Items, int Total)> GetPagedAsync(
        GovernmentEntityListQuery q,
        bool deleted,
        CancellationToken ct
    )
    {
        IQueryable<EntidadGubernamental> source = deleted
            ? db.EntidadesGubernamentales.IgnoreQueryFilters().Where(x => x.IsDeleted)
            : db.EntidadesGubernamentales;
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var search = string.Join(
                    ' ',
                    q.Search.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)
                )
                .ToUpperInvariant();
            source = source.Where(x => x.NombreNormalizado.Contains(search));
        }
        if (!string.IsNullOrWhiteSpace(q.Categoria))
            source = source.Where(x => x.Categoria == q.Categoria.Trim());
        if (!string.IsNullOrWhiteSpace(q.PoderDelEstado))
            source = source.Where(x => x.PoderDelEstado == q.PoderDelEstado.Trim());
        if (!string.IsNullOrWhiteSpace(q.Sector))
            source = source.Where(x => x.Sector == q.Sector.Trim());
        if (q.Activo.HasValue)
            source = source.Where(x => x.IsActive == q.Activo.Value);
        var total = await source.CountAsync(ct);
        source = q.Descending
            ? source.OrderByDescending(x => x.Nombre)
            : source.OrderBy(x => x.Nombre);
        return (
            await source
                .AsNoTracking()
                .Skip((q.PageNumber - 1) * q.PageSize)
                .Take(q.PageSize)
                .ToListAsync(ct),
            total
        );
    }

    public Task<EntidadGubernamental?> GetByIdAsync(Guid id, bool deleted, CancellationToken ct)
    {
        return (
            deleted ? db.EntidadesGubernamentales.IgnoreQueryFilters() : db.EntidadesGubernamentales
        ).SingleOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<bool> ActiveNameExistsAsync(string name, Guid? except, CancellationToken ct)
    {
        return db.EntidadesGubernamentales.AnyAsync(
            x => x.NombreNormalizado == name && (!except.HasValue || x.Id != except),
            ct
        );
    }

    public async Task<HashSet<string>> GetAllNormalizedNamesAsync(CancellationToken ct)
    {
        return (
            await db
                .EntidadesGubernamentales.IgnoreQueryFilters()
                .Select(x => x.NombreNormalizado)
                .ToListAsync(ct)
        ).ToHashSet(StringComparer.Ordinal);
    }

    public void Add(EntidadGubernamental entity)
    {
        db.Add(entity);
    }

    public void SetOriginalRowVersion(EntidadGubernamental entity, byte[] version)
    {
        db.Entry(entity).Property(x => x.RowVersion).OriginalValue = version;
    }
}
