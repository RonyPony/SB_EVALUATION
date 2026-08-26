using System.ComponentModel.DataAnnotations;
using SB.BACKEND.Domain.Support;
namespace SB.BACKEND.Application.Support;
public class CreateAreaRequest { [Required, StringLength(120)] public string Nombre { get; init; } = ""; [StringLength(500)] public string? Descripcion { get; init; } }
public sealed class UpdateAreaRequest : CreateAreaRequest { [Required] public string RowVersion { get; init; } = ""; }
public sealed class ChangeAreaStatusRequest { public bool Activo { get; init; } [Required] public string RowVersion { get; init; } = ""; }
public sealed record AreaResponse(Guid Id, string Nombre, string? Descripcion, bool Activo, string RowVersion);
public sealed class CreateSolicitudRequest
{
    [Required, StringLength(200)] public string Titulo { get; init; } = ""; [Required, StringLength(4000)] public string Descripcion { get; init; } = "";
    [EnumDataType(typeof(TipoSolicitud))] public TipoSolicitud Tipo { get; init; }
    [EnumDataType(typeof(PrioridadSolicitud))] public PrioridadSolicitud Prioridad { get; init; } = PrioridadSolicitud.Media;
    public Guid AreaId { get; init; } [StringLength(1000)] public string? ReferenciaEvidencia { get; init; } public DateTimeOffset? FechaCompromiso { get; init; }
}
public sealed class UpdateSolicitudRequest
{
    [Required, StringLength(200)] public string Titulo { get; init; } = ""; [Required, StringLength(4000)] public string Descripcion { get; init; } = "";
    [EnumDataType(typeof(TipoSolicitud))] public TipoSolicitud Tipo { get; init; } public Guid AreaId { get; init; }
    [StringLength(1000)] public string? ReferenciaEvidencia { get; init; } public DateTimeOffset? FechaCompromiso { get; init; }
    [Required] public string RowVersion { get; init; } = "";
}
public sealed class PatchSolicitudRequest
{
    [StringLength(200)] public string? Titulo { get; init; } [StringLength(4000)] public string? Descripcion { get; init; }
    public TipoSolicitud? Tipo { get; init; } public Guid? AreaId { get; init; } [StringLength(1000)] public string? ReferenciaEvidencia { get; init; }
    [Required] public string RowVersion { get; init; } = "";
}
public sealed class ChangePriorityRequest { [EnumDataType(typeof(PrioridadSolicitud))] public PrioridadSolicitud Prioridad { get; init; } [Required] public string RowVersion { get; init; } = ""; }
public sealed class AssignSolicitudRequest { public Guid ResponsableId { get; init; } [Required] public string RowVersion { get; init; } = ""; }
public sealed class ChangeSolicitudStatusRequest { [EnumDataType(typeof(EstadoSolicitud))] public EstadoSolicitud Estado { get; init; } [Required, StringLength(1000)] public string Comentario { get; init; } = ""; [StringLength(2000)] public string? ComentarioResolucion { get; init; } [Required] public string RowVersion { get; init; } = ""; }
public sealed class ReopenSolicitudRequest { [Required, StringLength(1000)] public string Comentario { get; init; } = ""; [Required] public string RowVersion { get; init; } = ""; }
public sealed class AddCommentRequest { [Required, StringLength(2000)] public string Contenido { get; init; } = ""; public bool EsInterno { get; init; } }
public sealed class UpdateCommentRequest { [Required, StringLength(2000)] public string Contenido { get; init; } = ""; [Required] public string RowVersion { get; init; } = ""; }
public sealed class SolicitudListQuery
{
    public string? Search { get; init; } public EstadoSolicitud? Estado { get; init; } public PrioridadSolicitud? Prioridad { get; init; }
    public Guid? AreaId { get; init; } public Guid? SolicitanteId { get; init; } public Guid? ResponsableId { get; init; } public bool? SinResponsable { get; init; }
    public TipoSolicitud? Tipo { get; init; } public DateTimeOffset? CreadaDesde { get; init; } public DateTimeOffset? CreadaHasta { get; init; }
    public DateTimeOffset? CompromisoDesde { get; init; } public DateTimeOffset? CompromisoHasta { get; init; } public bool? Vencidas { get; init; }
    public string OrderBy { get; init; } = "createdAt"; public bool Descending { get; init; } = true;
    [Range(1, int.MaxValue)] public int PageNumber { get; init; } = 1; [Range(1, 100)] public int PageSize { get; init; } = 20;
}
public sealed record SolicitudResponse(Guid Id, string Codigo, string Titulo, string Descripcion, TipoSolicitud Tipo, PrioridadSolicitud Prioridad,
    EstadoSolicitud Estado, Guid AreaId, string Area, Guid SolicitanteId, Guid? ResponsableId, string? Responsable,
    string? ReferenciaEvidencia, DateTimeOffset? FechaCompromiso, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt, string RowVersion);
public sealed record HistorialResponse(Guid Id, EstadoSolicitud EstadoAnterior, EstadoSolicitud EstadoNuevo, string Comentario, Guid UsuarioId, DateTimeOffset CreatedAt);
public sealed record ComentarioResponse(Guid Id, Guid UsuarioId, string Contenido, bool EsInterno, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt, string RowVersion);
public sealed record NotificacionResponse(Guid Id, Guid SolicitudId, TipoNotificacion Tipo, string Titulo, string Mensaje, bool Leida, DateTimeOffset? FechaLectura, DateTimeOffset CreatedAt);
public sealed record DashboardItem(Guid Id, string Codigo, string Titulo, EstadoSolicitud Estado, string? Responsable, DateTimeOffset CreatedAt, DateTimeOffset? FechaCompromiso);
public sealed record DashboardResponse(int Abiertas, int Cerradas, int Vencidas, IReadOnlyDictionary<EstadoSolicitud,int> PorEstado, IReadOnlyDictionary<PrioridadSolicitud,int> PorPrioridad, IReadOnlyCollection<DashboardItem> Ultimas);
