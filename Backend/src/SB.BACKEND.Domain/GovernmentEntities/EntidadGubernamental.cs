using SB.BACKEND.Domain.Common;

namespace SB.BACKEND.Domain.GovernmentEntities;

public sealed class EntidadGubernamental : AuditableEntity
{
    private EntidadGubernamental() { }
    public EntidadGubernamental(string nombre, string nombreNormalizado, string categoria, string poderDelEstado, string sector)
    { Nombre = nombre; NombreNormalizado = nombreNormalizado; Categoria = categoria; PoderDelEstado = poderDelEstado; Sector = sector; }
    public string Nombre { get; private set; } = string.Empty;
    public string NombreNormalizado { get; private set; } = string.Empty;
    public string Categoria { get; private set; } = string.Empty;
    public string PoderDelEstado { get; private set; } = string.Empty;
    public string Sector { get; private set; } = string.Empty;
    public void Actualizar(string nombre, string normalizado, string categoria, string poder, string sector)
    { Nombre = nombre; NombreNormalizado = normalizado; Categoria = categoria; PoderDelEstado = poder; Sector = sector; }
    public void CambiarEstado(bool activo) { if (IsDeleted) throw new InvalidOperationException(); IsActive = activo; }
    public void Eliminar(DateTimeOffset now, Guid? userId) { if (IsDeleted) throw new InvalidOperationException(); SoftDelete(now, userId); }
    public void Restaurar(DateTimeOffset now, Guid? userId) { if (!IsDeleted) throw new InvalidOperationException(); Restore(now, userId); }
}
