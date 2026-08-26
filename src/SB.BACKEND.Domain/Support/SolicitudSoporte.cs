using SB.BACKEND.Domain.Common;
using SB.BACKEND.Domain.Security;
namespace SB.BACKEND.Domain.Support;

public sealed class SolicitudSoporte : AuditableEntity
{
    private static readonly IReadOnlyDictionary<EstadoSolicitud, EstadoSolicitud[]> Transitions = new Dictionary<EstadoSolicitud, EstadoSolicitud[]>
    {
        [EstadoSolicitud.Registrada] = [EstadoSolicitud.EnAnalisis],
        [EstadoSolicitud.EnAnalisis] = [EstadoSolicitud.EnProgreso],
        [EstadoSolicitud.EnProgreso] = [EstadoSolicitud.EnEsperaSolicitante, EstadoSolicitud.Resuelta],
        [EstadoSolicitud.EnEsperaSolicitante] = [EstadoSolicitud.EnProgreso],
        [EstadoSolicitud.Resuelta] = [EstadoSolicitud.EnProgreso, EstadoSolicitud.Cerrada],
        [EstadoSolicitud.Cerrada] = []
    };
    private SolicitudSoporte() { }
    public SolicitudSoporte(string codigo, string titulo, string descripcion, TipoSolicitud tipo, PrioridadSolicitud prioridad,
        Guid areaId, Guid solicitanteId, string? evidencia, DateTimeOffset? compromiso)
    { Codigo = codigo; Titulo = titulo; Descripcion = descripcion; Tipo = tipo; Prioridad = prioridad; Estado = EstadoSolicitud.Registrada; AreaId = areaId; SolicitanteId = solicitanteId; ReferenciaEvidencia = evidencia; FechaCompromiso = compromiso?.ToUniversalTime(); }
    public string Codigo { get; private set; } = ""; public string Titulo { get; private set; } = ""; public string Descripcion { get; private set; } = "";
    public TipoSolicitud Tipo { get; private set; }
    public PrioridadSolicitud Prioridad { get; private set; }
    public EstadoSolicitud Estado { get; private set; }
    public Guid AreaId { get; private set; }
    public Area Area { get; private set; } = null!;
    public Guid SolicitanteId { get; private set; }
    public User Solicitante { get; private set; } = null!;
    public Guid? ResponsableId { get; private set; }
    public User? Responsable { get; private set; }
    public string? ReferenciaEvidencia { get; private set; }
    public DateTimeOffset? FechaCompromiso { get; private set; }
    public DateTimeOffset? FechaAsignacion { get; private set; }
    public DateTimeOffset? FechaInicioAtencion { get; private set; }
    public DateTimeOffset? FechaResolucion { get; private set; }
    public DateTimeOffset? FechaCierre { get; private set; }
    public string? ComentarioResolucion { get; private set; }
    public ICollection<HistorialSolicitud> Historial { get; private set; } = new List<HistorialSolicitud>();
    public ICollection<ComentarioSolicitud> Comentarios { get; private set; } = new List<ComentarioSolicitud>();
    public ICollection<Notificacion> Notificaciones { get; private set; } = new List<Notificacion>();
    public void Editar(string titulo, string descripcion, TipoSolicitud tipo, Guid areaId, string? evidencia, DateTimeOffset? compromiso)
    { if (Estado == EstadoSolicitud.Cerrada || IsDeleted) throw new InvalidOperationException(); Titulo = titulo; Descripcion = descripcion; Tipo = tipo; AreaId = areaId; ReferenciaEvidencia = evidencia; FechaCompromiso = compromiso?.ToUniversalTime(); }
    public void CambiarPrioridad(PrioridadSolicitud prioridad) { if (IsDeleted || Estado == EstadoSolicitud.Cerrada) throw new InvalidOperationException(); Prioridad = prioridad; }
    public Guid? Asignar(Guid responsableId, Guid usuarioId, DateTimeOffset now) { if (IsDeleted || Estado == EstadoSolicitud.Cerrada) throw new InvalidOperationException(); var previous = ResponsableId; ResponsableId = responsableId; FechaAsignacion = now.ToUniversalTime(); Historial.Add(new HistorialSolicitud(Id, Estado, Estado, $"Asignación: {previous?.ToString() ?? "Sin responsable"} -> {responsableId}", usuarioId)); return previous; }
    public EstadoSolicitud CambiarEstado(EstadoSolicitud nuevo, string comentario, Guid usuarioId, DateTimeOffset now)
    {
        if (IsDeleted || !Transitions[Estado].Contains(nuevo)) throw new InvalidOperationException();
        if (nuevo == EstadoSolicitud.Cerrada && string.IsNullOrWhiteSpace(ComentarioResolucion)) throw new InvalidOperationException();
        var anterior = Estado; Estado = nuevo;
        if (nuevo == EstadoSolicitud.EnProgreso && FechaInicioAtencion is null) FechaInicioAtencion = now.ToUniversalTime();
        if (nuevo == EstadoSolicitud.Resuelta) FechaResolucion = now.ToUniversalTime();
        if (nuevo == EstadoSolicitud.Cerrada) FechaCierre = now.ToUniversalTime();
        Historial.Add(new HistorialSolicitud(Id, anterior, nuevo, comentario, usuarioId)); return anterior;
    }
    public void RegistrarResolucion(string comentario) { if (string.IsNullOrWhiteSpace(comentario)) throw new ArgumentException(); ComentarioResolucion = comentario; }
    public void Reabrir(string comentario, Guid usuarioId, DateTimeOffset now)
    { if (IsDeleted || Estado != EstadoSolicitud.Cerrada) throw new InvalidOperationException(); var anterior = Estado; Estado = EstadoSolicitud.EnProgreso; FechaCierre = null; Historial.Add(new HistorialSolicitud(Id, anterior, Estado, comentario, usuarioId)); }
    public void Eliminar(DateTimeOffset now, Guid? userId) { if (IsDeleted) throw new InvalidOperationException(); SoftDelete(now, userId); }
}
