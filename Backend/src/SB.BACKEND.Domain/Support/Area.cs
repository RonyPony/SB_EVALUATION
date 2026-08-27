using SB.BACKEND.Domain.Common;

namespace SB.BACKEND.Domain.Support;

public sealed class Area : AuditableEntity
{
    private Area() { }

    public Area(string nombre, string normalizado, string? descripcion)
    {
        Nombre = nombre;
        NombreNormalizado = normalizado;
        Descripcion = descripcion;
    }

    public string Nombre { get; private set; } = "";
    public string NombreNormalizado { get; private set; } = "";
    public string? Descripcion { get; private set; }
    public ICollection<SolicitudSoporte> Solicitudes { get; private set; } = [];

    public void Actualizar(string nombre, string normalizado, string? descripcion)
    {
        Nombre = nombre;
        NombreNormalizado = normalizado;
        Descripcion = descripcion;
    }

    public void CambiarEstado(bool activo)
    {
        if (IsDeleted)
            throw new InvalidOperationException();
        IsActive = activo;
    }

    public void Eliminar(DateTimeOffset now, Guid? userId)
    {
        if (IsDeleted)
            throw new InvalidOperationException();
        SoftDelete(now, userId);
    }

    public void Restaurar(DateTimeOffset now, Guid? userId)
    {
        if (!IsDeleted)
            throw new InvalidOperationException();
        Restore(now, userId);
    }
}
