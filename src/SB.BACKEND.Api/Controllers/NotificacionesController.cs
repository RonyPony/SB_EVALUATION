using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.BACKEND.Application.Support;
namespace SB.BACKEND.Api.Controllers;
[ApiController,Route("api/notificaciones"),Authorize]
public sealed class NotificacionesController(INotificationApplicationService service):ControllerBase
{
    [HttpGet] public Task<IReadOnlyCollection<NotificacionResponse>> Get(CancellationToken ct)=>service.GetMineAsync(ct);
    [HttpGet("no-leidas/count")] public async Task<object> Count(CancellationToken ct)=>new{count=await service.UnreadCountAsync(ct)};
    [HttpPatch("{id:guid}/leida")] public async Task<IActionResult> Read(Guid id,CancellationToken ct){await service.MarkReadAsync(id,ct);return NoContent();}
    [HttpPatch("leer-todas")] public async Task<IActionResult> ReadAll(CancellationToken ct){await service.MarkAllReadAsync(ct);return NoContent();}
}
