using SB.BACKEND.Application.Common;
using SB.BACKEND.Application.GovernmentEntities;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Domain.GovernmentEntities;
namespace SB.BACKEND.Services.GovernmentEntities;
internal sealed class GovernmentEntityService(IGovernmentEntityRepository repository, IUnitOfWork unitOfWork, ICurrentUserService currentUser) : IGovernmentEntityService
{
    public Task<PagedResult<GovernmentEntityResponse>> GetAllAsync(GovernmentEntityListQuery q, CancellationToken ct) => Page(q, false, ct);
    public Task<PagedResult<GovernmentEntityResponse>> GetDeletedAsync(GovernmentEntityListQuery q, CancellationToken ct) => Page(q, true, ct);
    public async Task<GovernmentEntityResponse> GetByIdAsync(Guid id, CancellationToken ct) { ValidId(id); return Map(await Find(id, false, ct)); }
    public async Task<GovernmentEntityResponse> CreateAsync(CreateGovernmentEntityRequest request, CancellationToken ct)
    {
        var v = Values(request); var normalized = Normalize(v.Name);
        if (await repository.ActiveNameExistsAsync(normalized, null, ct)) throw new ConflictException("Ya existe una entidad gubernamental activa con ese nombre.");
        var entity = new EntidadGubernamental(v.Name, normalized, v.Category, v.Power, v.Sector); repository.Add(entity); await unitOfWork.SaveChangesAsync(ct); return Map(entity);
    }
    public async Task<GovernmentEntityResponse> UpdateAsync(Guid id, UpdateGovernmentEntityRequest request, CancellationToken ct)
    {
        ValidId(id); var entity = await Find(id, false, ct); var v = Values(request); var normalized = Normalize(v.Name);
        if (await repository.ActiveNameExistsAsync(normalized, id, ct)) throw new ConflictException("Ya existe una entidad gubernamental activa con ese nombre.");
        Version(entity, request.RowVersion); entity.Actualizar(v.Name, normalized, v.Category, v.Power, v.Sector); await unitOfWork.SaveChangesAsync(ct); return Map(entity);
    }
    public async Task ChangeStatusAsync(Guid id, ChangeGovernmentEntityStatusRequest request, CancellationToken ct)
    {
        ValidId(id); var entity = await Find(id, false, ct);
        if (entity.IsActive == request.Activo) throw new ValidationException($"La entidad ya está {(request.Activo ? "activa" : "inactiva")}.");
        Version(entity, request.RowVersion); entity.CambiarEstado(request.Activo); await unitOfWork.SaveChangesAsync(ct);
    }
    public async Task DeleteAsync(Guid id, string version, CancellationToken ct)
    { ValidId(id); var entity = await Find(id, false, ct); Version(entity, version); entity.Eliminar(DateTimeOffset.UtcNow, currentUser.UserId); await unitOfWork.SaveChangesAsync(ct); }
    public async Task RestoreAsync(Guid id, string version, CancellationToken ct)
    {
        ValidId(id); var entity = await Find(id, true, ct); if (!entity.IsDeleted) throw new ValidationException("La entidad no está eliminada.");
        if (await repository.ActiveNameExistsAsync(entity.NombreNormalizado, id, ct)) throw new ConflictException("Existe una entidad activa con el mismo nombre.");
        Version(entity, version); entity.Restaurar(DateTimeOffset.UtcNow, currentUser.UserId); await unitOfWork.SaveChangesAsync(ct);
    }
    private async Task<PagedResult<GovernmentEntityResponse>> Page(GovernmentEntityListQuery q, bool deleted, CancellationToken ct)
    { var (items, total) = await repository.GetPagedAsync(q, deleted, ct); return new(items.Select(Map).ToArray(), q.PageNumber, q.PageSize, total); }
    private async Task<EntidadGubernamental> Find(Guid id, bool deleted, CancellationToken ct) => await repository.GetByIdAsync(id, deleted, ct) ?? throw new NotFoundException("Entidad gubernamental no encontrada.");
    private void Version(EntidadGubernamental e, string value) { try { repository.SetOriginalRowVersion(e, Convert.FromBase64String(value)); } catch (FormatException) { throw new ValidationException("RowVersion debe ser Base64 válido."); } }
    private static void ValidId(Guid id) { if (id == Guid.Empty) throw new ValidationException("El identificador no es válido."); }
    private static (string Name, string Category, string Power, string Sector) Values(CreateGovernmentEntityRequest r)
    {
        var v = (r.Nombre?.Trim() ?? "", r.Categoria?.Trim() ?? "", r.PoderDelEstado?.Trim() ?? "", r.Sector?.Trim() ?? "");
        if (string.IsNullOrWhiteSpace(v.Item1) || string.IsNullOrWhiteSpace(v.Item2) || string.IsNullOrWhiteSpace(v.Item3) || string.IsNullOrWhiteSpace(v.Item4)) throw new ValidationException("Todos los campos son obligatorios.");
        if (v.Item1.Length > GovernmentEntityLengths.Name || v.Item2.Length > GovernmentEntityLengths.Category || v.Item3.Length > GovernmentEntityLengths.StatePower || v.Item4.Length > GovernmentEntityLengths.Sector) throw new ValidationException("Uno o más campos exceden la longitud máxima.");
        return v;
    }
    internal static string Normalize(string value) => string.Join(' ', value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToUpperInvariant();
    private static GovernmentEntityResponse Map(EntidadGubernamental x) => new(x.Id, x.Nombre, x.Categoria, x.PoderDelEstado, x.Sector, x.IsActive, x.IsDeleted, x.CreatedAt, x.UpdatedAt, x.DeletedAt, x.CreatedBy, x.UpdatedBy, x.DeletedBy, Convert.ToBase64String(x.RowVersion));
}
