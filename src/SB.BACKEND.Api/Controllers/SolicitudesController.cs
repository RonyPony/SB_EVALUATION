using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.BACKEND.Application.Common;
using SB.BACKEND.Application.Support;
namespace SB.BACKEND.Api.Controllers;
[ApiController,Route("api/solicitudes"),Authorize]
public sealed class SolicitudesController(ISolicitudService service):ControllerBase
{
    [HttpGet] public Task<PagedResult<SolicitudResponse>> GetAll([FromQuery]SolicitudListQuery query,CancellationToken ct)=>service.GetAllAsync(query,ct);
    [HttpGet("{id:guid}")] public Task<SolicitudResponse> GetById(Guid id,CancellationToken ct)=>service.GetByIdAsync(id,ct);
    [HttpPost] public async Task<ActionResult<SolicitudResponse>> Create(CreateSolicitudRequest request,CancellationToken ct){var result=await service.CreateAsync(request,ct);return CreatedAtAction(nameof(GetById),new{id=result.Id},result);}
    [HttpPut("{id:guid}")] public Task<SolicitudResponse> Update(Guid id,UpdateSolicitudRequest request,CancellationToken ct)=>service.UpdateAsync(id,request,ct);
    [HttpPatch("{id:guid}")] public Task<SolicitudResponse> Patch(Guid id,PatchSolicitudRequest request,CancellationToken ct)=>service.PatchAsync(id,request,ct);
    [HttpPatch("{id:guid}/prioridad"),Authorize(Policy="SupportStaff")] public async Task<IActionResult> Priority(Guid id,ChangePriorityRequest request,CancellationToken ct){await service.ChangePriorityAsync(id,request,ct);return NoContent();}
    [HttpPatch("{id:guid}/asignacion"),Authorize(Policy="SupportStaff")] public async Task<IActionResult> Assign(Guid id,AssignSolicitudRequest request,CancellationToken ct){await service.AssignAsync(id,request,ct);return NoContent();}
    [HttpPatch("{id:guid}/estado"),Authorize(Policy="SupportStaff")] public async Task<IActionResult> State(Guid id,ChangeSolicitudStatusRequest request,CancellationToken ct){await service.ChangeStatusAsync(id,request,ct);return NoContent();}
    [HttpPatch("{id:guid}/reabrir")] public async Task<IActionResult> Reopen(Guid id,ReopenSolicitudRequest request,CancellationToken ct){await service.ReopenAsync(id,request,ct);return NoContent();}
    [HttpDelete("{id:guid}"),Authorize(Policy="SupportAdmin")] public async Task<IActionResult> Delete(Guid id,[FromQuery]string rowVersion,CancellationToken ct){await service.DeleteAsync(id,rowVersion,ct);return NoContent();}
    [HttpGet("{id:guid}/historial")] public Task<IReadOnlyCollection<HistorialResponse>> History(Guid id,CancellationToken ct)=>service.GetHistoryAsync(id,ct);
    [HttpGet("{id:guid}/comentarios")] public Task<IReadOnlyCollection<ComentarioResponse>> Comments(Guid id,CancellationToken ct)=>service.GetCommentsAsync(id,ct);
    [HttpPost("{id:guid}/comentarios")] public async Task<ActionResult<ComentarioResponse>> Comment(Guid id,AddCommentRequest request,CancellationToken ct){var result=await service.AddCommentAsync(id,request,ct);return Created($"api/solicitudes/{id}/comentarios/{result.Id}",result);}
    [HttpPatch("{id:guid}/comentarios/{commentId:guid}")] public Task<ComentarioResponse> UpdateComment(Guid id,Guid commentId,UpdateCommentRequest request,CancellationToken ct)=>service.UpdateCommentAsync(id,commentId,request,ct);
    [HttpDelete("{id:guid}/comentarios/{commentId:guid}")] public async Task<IActionResult> DeleteComment(Guid id,Guid commentId,[FromQuery]string rowVersion,CancellationToken ct){await service.DeleteCommentAsync(id,commentId,rowVersion,ct);return NoContent();}
}
