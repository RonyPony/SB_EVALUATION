using SB.BACKEND.Domain.Common;

namespace SB.BACKEND.Domain.Support;

public sealed class ComentarioSolicitud : AuditableEntity
{
    private ComentarioSolicitud() { }

    public ComentarioSolicitud(Guid solicitudId, Guid usuarioId, string contenido, bool interno)
    {
        SolicitudId = solicitudId;
        UsuarioId = usuarioId;
        Contenido = contenido;
        EsInterno = interno;
    }

    public Guid SolicitudId { get; private set; }
    public SolicitudSoporte Solicitud { get; private set; } = null!;
    public Guid UsuarioId { get; private set; }
    public string Contenido { get; private set; } = "";
    public bool EsInterno { get; private set; }

    public void Editar(string contenido)
    {
        if (IsDeleted)
            throw new InvalidOperationException();
        Contenido = contenido;
    }

    public void Eliminar(DateTimeOffset now, Guid? userId)
    {
        if (IsDeleted)
            throw new InvalidOperationException();
        SoftDelete(now, userId);
    }
}
