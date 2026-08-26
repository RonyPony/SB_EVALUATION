using System.Data;
using Microsoft.EntityFrameworkCore;
using SB.BACKEND.Application.Support;
using SB.BACKEND.Domain.Common;
using SB.BACKEND.Domain.Support;
namespace SB.BACKEND.Infrastructure.Persistence.Repositories;
internal sealed class SupportRepository(SecurityDbContext db) : ISupportRepository
{
    public Task<Area?> GetAreaAsync(Guid id, CancellationToken ct) => db.Areas.SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task<IReadOnlyCollection<Area>> GetAreasAsync(bool activeOnly, CancellationToken ct) => await db.Areas.AsNoTracking().Where(x => !activeOnly || x.IsActive).OrderBy(x => x.Nombre).ToListAsync(ct);
    public Task<bool> AreaNameExistsAsync(string name, Guid? except, CancellationToken ct) => db.Areas.AnyAsync(x => x.NombreNormalizado == name && (!except.HasValue || x.Id != except), ct);
    public void AddArea(Area area) => db.Areas.Add(area);
    public async Task<string> NextCodeAsync(CancellationToken ct)
    {
        var connection = db.Database.GetDbConnection(); var close = connection.State != ConnectionState.Open;
        if (close) await connection.OpenAsync(ct);
        try { await using var command = connection.CreateCommand(); command.CommandText = "SELECT NEXT VALUE FOR [SolicitudCodeSequence]"; var value = Convert.ToInt64(await command.ExecuteScalarAsync(ct)); return $"SOL-{DateTimeOffset.UtcNow.Year}-{value:000000}"; }
        finally { if (close) await connection.CloseAsync(); }
    }
    public void AddSolicitud(SolicitudSoporte request) => db.SolicitudesSoporte.Add(request);
    public Task<SolicitudSoporte?> GetSolicitudAsync(Guid id, CancellationToken ct) => db.SolicitudesSoporte.Include(x => x.Area).Include(x => x.Solicitante).Include(x => x.Responsable).SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task<(IReadOnlyCollection<SolicitudSoporte> Items, int Total)> GetSolicitudesAsync(SolicitudListQuery q, Guid userId, IReadOnlyCollection<string> roles, CancellationToken ct)
    {
        var source = Scope(db.SolicitudesSoporte.AsNoTracking().Include(x => x.Area).Include(x => x.Responsable), userId, roles);
        if (!string.IsNullOrWhiteSpace(q.Search)) { var s = q.Search.Trim(); source = source.Where(x => x.Codigo.Contains(s) || x.Titulo.Contains(s) || x.Descripcion.Contains(s)); }
        if (q.Estado.HasValue) source = source.Where(x => x.Estado == q.Estado); if (q.Prioridad.HasValue) source = source.Where(x => x.Prioridad == q.Prioridad);
        if (q.AreaId.HasValue) source = source.Where(x => x.AreaId == q.AreaId); if (q.SolicitanteId.HasValue) source = source.Where(x => x.SolicitanteId == q.SolicitanteId);
        if (q.ResponsableId.HasValue) source = source.Where(x => x.ResponsableId == q.ResponsableId); if (q.SinResponsable == true) source = source.Where(x => x.ResponsableId == null);
        if (q.Tipo.HasValue) source = source.Where(x => x.Tipo == q.Tipo); if (q.CreadaDesde.HasValue) source = source.Where(x => x.CreatedAt >= q.CreadaDesde.Value);
        if (q.CreadaHasta.HasValue) source = source.Where(x => x.CreatedAt <= q.CreadaHasta.Value); if (q.CompromisoDesde.HasValue) source = source.Where(x => x.FechaCompromiso >= q.CompromisoDesde.Value);
        if (q.CompromisoHasta.HasValue) source = source.Where(x => x.FechaCompromiso <= q.CompromisoHasta.Value); if (q.Vencidas == true) source = source.Where(x => x.FechaCompromiso < DateTimeOffset.UtcNow && x.Estado != EstadoSolicitud.Cerrada);
        var total = await source.CountAsync(ct);
        source = q.OrderBy.ToLowerInvariant() switch { "codigo" => q.Descending ? source.OrderByDescending(x => x.Codigo) : source.OrderBy(x => x.Codigo), "prioridad" => q.Descending ? source.OrderByDescending(x => x.Prioridad) : source.OrderBy(x => x.Prioridad), "estado" => q.Descending ? source.OrderByDescending(x => x.Estado) : source.OrderBy(x => x.Estado), "compromiso" => q.Descending ? source.OrderByDescending(x => x.FechaCompromiso) : source.OrderBy(x => x.FechaCompromiso), _ => q.Descending ? source.OrderByDescending(x => x.CreatedAt) : source.OrderBy(x => x.CreatedAt) };
        return (await source.Skip((q.PageNumber - 1) * q.PageSize).Take(q.PageSize).ToListAsync(ct), total);
    }
    public Task<bool> IsEligibleResponsibleAsync(Guid userId, CancellationToken ct) => db.Users.AnyAsync(x => x.Id == userId && x.IsActive && x.UserRoles.Any(r => r.Role.Name == "Analista" || r.Role.Name == "Administrador" || r.Role.Name == "Admin"), ct);
    public async Task<IReadOnlyCollection<HistorialSolicitud>> GetHistoryAsync(Guid id, CancellationToken ct) => await db.HistorialSolicitudes.AsNoTracking().Where(x => x.SolicitudId == id).OrderBy(x => x.CreatedAt).ToListAsync(ct);
    public void AddHistory(HistorialSolicitud history) => db.Entry(history).State = EntityState.Added;
    public async Task<IReadOnlyCollection<ComentarioSolicitud>> GetCommentsAsync(Guid id, bool includeInternal, CancellationToken ct) => await db.ComentariosSolicitud.AsNoTracking().Where(x => x.SolicitudId == id && (includeInternal || !x.EsInterno)).OrderBy(x => x.CreatedAt).ToListAsync(ct);
    public void AddComment(ComentarioSolicitud comment) => db.ComentariosSolicitud.Add(comment);
    public Task<ComentarioSolicitud?> GetCommentAsync(Guid requestId, Guid commentId, CancellationToken ct) => db.ComentariosSolicitud.SingleOrDefaultAsync(x => x.SolicitudId == requestId && x.Id == commentId, ct);
    public async Task<IReadOnlyCollection<Notificacion>> GetNotificationsAsync(Guid userId, CancellationToken ct) => await db.Notificaciones.AsNoTracking().Where(x => x.UsuarioId == userId).OrderByDescending(x => x.CreatedAt).Take(100).ToListAsync(ct);
    public Task<Notificacion?> GetNotificationAsync(Guid id, Guid userId, CancellationToken ct) => db.Notificaciones.SingleOrDefaultAsync(x => x.Id == id && x.UsuarioId == userId, ct);
    public Task<int> UnreadCountAsync(Guid userId, CancellationToken ct) => db.Notificaciones.CountAsync(x => x.UsuarioId == userId && !x.Leida, ct);
    public async Task<IReadOnlyCollection<Notificacion>> GetUnreadAsync(Guid userId, CancellationToken ct) => await db.Notificaciones.Where(x => x.UsuarioId == userId && !x.Leida).ToListAsync(ct);
    public void AddNotification(Notificacion notification) => db.Notificaciones.Add(notification);
    public async Task<DashboardResponse> GetDashboardAsync(Guid userId, IReadOnlyCollection<string> roles, CancellationToken ct)
    {
        var source = Scope(db.SolicitudesSoporte.AsNoTracking().Include(x => x.Responsable), userId, roles); var now = DateTimeOffset.UtcNow;
        var porEstado = await source.GroupBy(x => x.Estado).ToDictionaryAsync(x => x.Key, x => x.Count(), ct); var porPrioridad = await source.GroupBy(x => x.Prioridad).ToDictionaryAsync(x => x.Key, x => x.Count(), ct);
        var abiertas = await source.CountAsync(x => x.Estado != EstadoSolicitud.Cerrada, ct); var cerradas = await source.CountAsync(x => x.Estado == EstadoSolicitud.Cerrada, ct); var vencidas = await source.CountAsync(x => x.FechaCompromiso < now && x.Estado != EstadoSolicitud.Cerrada, ct);
        var ultimas = await source.OrderByDescending(x => x.CreatedAt).Take(10).Select(x => new DashboardItem(x.Id, x.Codigo, x.Titulo, x.Estado, x.Responsable == null ? null : x.Responsable.Username, x.CreatedAt, x.FechaCompromiso)).ToListAsync(ct);
        return new DashboardResponse(abiertas, cerradas, vencidas, porEstado, porPrioridad, ultimas);
    }
    public void SetVersion(AuditableEntity entity, byte[] version) => db.Entry(entity).Property(x => x.RowVersion).OriginalValue = version;
    private static IQueryable<SolicitudSoporte> Scope(IQueryable<SolicitudSoporte> source, Guid userId, IReadOnlyCollection<string> roles)
    { if (roles.Contains("Admin") || roles.Contains("Administrador")) return source; if (roles.Contains("Analista")) return source.Where(x => x.ResponsableId == userId || x.ResponsableId == null); return source.Where(x => x.SolicitanteId == userId); }
}
