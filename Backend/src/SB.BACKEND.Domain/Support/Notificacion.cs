using SB.BACKEND.Domain.Common;

namespace SB.BACKEND.Domain.Support;

public sealed class Notificacion : BaseEntity
{
    private Notificacion() { }

    public Notificacion(
        Guid usuarioId,
        Guid solicitudId,
        TipoNotificacion tipo,
        string titulo,
        string mensaje
    )
    {
        UsuarioId = usuarioId;
        SolicitudId = solicitudId;
        Tipo = tipo;
        Titulo = titulo;
        Mensaje = mensaje;
    }

    public Guid UsuarioId { get; private set; }
    public Guid SolicitudId { get; private set; }
    public SolicitudSoporte Solicitud { get; private set; } = null!;
    public TipoNotificacion Tipo { get; private set; }
    public string Titulo { get; private set; } = "";
    public string Mensaje { get; private set; } = "";
    public bool Leida { get; private set; }
    public DateTimeOffset? FechaLectura { get; private set; }

    public void MarcarLeida(DateTimeOffset now)
    {
        Leida = true;
        FechaLectura = now.ToUniversalTime();
    }
}
