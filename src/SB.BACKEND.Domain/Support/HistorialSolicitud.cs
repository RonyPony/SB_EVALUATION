using SB.BACKEND.Domain.Common;
namespace SB.BACKEND.Domain.Support;
public sealed class HistorialSolicitud : BaseEntity
{
    private HistorialSolicitud() { }
    public HistorialSolicitud(Guid solicitudId, EstadoSolicitud anterior, EstadoSolicitud nuevo, string comentario, Guid usuarioId)
    { SolicitudId = solicitudId; EstadoAnterior = anterior; EstadoNuevo = nuevo; Comentario = comentario; UsuarioId = usuarioId; }
    public Guid SolicitudId { get; private set; } public SolicitudSoporte Solicitud { get; private set; } = null!;
    public EstadoSolicitud EstadoAnterior { get; private set; } public EstadoSolicitud EstadoNuevo { get; private set; }
    public string Comentario { get; private set; } = ""; public Guid UsuarioId { get; private set; }
}
