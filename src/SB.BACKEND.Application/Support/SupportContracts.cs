using SB.BACKEND.Application.Common;
using SB.BACKEND.Domain.Common;
using SB.BACKEND.Domain.Support;
namespace SB.BACKEND.Application.Support;
public interface IAreaService
{
    Task<IReadOnlyCollection<AreaResponse>> GetAllAsync(bool activeOnly, CancellationToken ct); Task<AreaResponse> GetByIdAsync(Guid id, CancellationToken ct);
    Task<AreaResponse> CreateAsync(CreateAreaRequest request, CancellationToken ct); Task<AreaResponse> UpdateAsync(Guid id, UpdateAreaRequest request, CancellationToken ct);
    Task ChangeStatusAsync(Guid id, ChangeAreaStatusRequest request, CancellationToken ct); Task DeleteAsync(Guid id, string version, CancellationToken ct);
}
public interface ISolicitudService
{
    Task<SolicitudResponse> CreateAsync(CreateSolicitudRequest request, CancellationToken ct); Task<PagedResult<SolicitudResponse>> GetAllAsync(SolicitudListQuery query, CancellationToken ct);
    Task<SolicitudResponse> GetByIdAsync(Guid id, CancellationToken ct); Task<SolicitudResponse> UpdateAsync(Guid id, UpdateSolicitudRequest request, CancellationToken ct);
    Task<SolicitudResponse> PatchAsync(Guid id, PatchSolicitudRequest request, CancellationToken ct);
    Task ChangePriorityAsync(Guid id, ChangePriorityRequest request, CancellationToken ct); Task AssignAsync(Guid id, AssignSolicitudRequest request, CancellationToken ct);
    Task ChangeStatusAsync(Guid id, ChangeSolicitudStatusRequest request, CancellationToken ct); Task ReopenAsync(Guid id, ReopenSolicitudRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, string version, CancellationToken ct); Task<IReadOnlyCollection<HistorialResponse>> GetHistoryAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<ComentarioResponse>> GetCommentsAsync(Guid id, CancellationToken ct); Task<ComentarioResponse> AddCommentAsync(Guid id, AddCommentRequest request, CancellationToken ct);
    Task<ComentarioResponse> UpdateCommentAsync(Guid id, Guid commentId, UpdateCommentRequest request, CancellationToken ct); Task DeleteCommentAsync(Guid id, Guid commentId, string version, CancellationToken ct);
}
public interface INotificationApplicationService { Task<IReadOnlyCollection<NotificacionResponse>> GetMineAsync(CancellationToken ct); Task<int> UnreadCountAsync(CancellationToken ct); Task MarkReadAsync(Guid id, CancellationToken ct); Task MarkAllReadAsync(CancellationToken ct); }
public interface IDashboardService { Task<DashboardResponse> GetAsync(CancellationToken ct); }
public interface INotificationService { void Add(Guid userId, Guid requestId, TipoNotificacion type, string title, string message); }
public interface ISupportRepository
{
    Task<Area?> GetAreaAsync(Guid id, CancellationToken ct); Task<IReadOnlyCollection<Area>> GetAreasAsync(bool activeOnly, CancellationToken ct); Task<bool> AreaNameExistsAsync(string name, Guid? except, CancellationToken ct); void AddArea(Area area);
    Task<string> NextCodeAsync(CancellationToken ct); void AddSolicitud(SolicitudSoporte request); Task<SolicitudSoporte?> GetSolicitudAsync(Guid id, CancellationToken ct);
    Task<(IReadOnlyCollection<SolicitudSoporte> Items,int Total)> GetSolicitudesAsync(SolicitudListQuery query, Guid userId, IReadOnlyCollection<string> roles, CancellationToken ct);
    Task<bool> IsEligibleResponsibleAsync(Guid userId, CancellationToken ct); Task<IReadOnlyCollection<HistorialSolicitud>> GetHistoryAsync(Guid id, CancellationToken ct);
    void AddHistory(HistorialSolicitud history);
    Task<IReadOnlyCollection<ComentarioSolicitud>> GetCommentsAsync(Guid id, bool includeInternal, CancellationToken ct); void AddComment(ComentarioSolicitud comment);
    Task<ComentarioSolicitud?> GetCommentAsync(Guid requestId, Guid commentId, CancellationToken ct);
    Task<IReadOnlyCollection<Notificacion>> GetNotificationsAsync(Guid userId, CancellationToken ct); Task<Notificacion?> GetNotificationAsync(Guid id, Guid userId, CancellationToken ct);
    Task<int> UnreadCountAsync(Guid userId, CancellationToken ct); Task<IReadOnlyCollection<Notificacion>> GetUnreadAsync(Guid userId, CancellationToken ct); void AddNotification(Notificacion notification);
    Task<DashboardResponse> GetDashboardAsync(Guid userId, IReadOnlyCollection<string> roles, CancellationToken ct); void SetVersion(AuditableEntity entity, byte[] version);
}
