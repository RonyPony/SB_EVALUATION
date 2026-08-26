using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.BACKEND.Application.Common;
using SB.BACKEND.Application.GovernmentEntities;
using SB.BACKEND.Application.Security;
namespace SB.BACKEND.Api.Controllers;
[ApiController, Authorize, Route("api/entidades-gubernamentales")]
public sealed class EntidadesGubernamentalesController(IGovernmentEntityService service) : ControllerBase
{
    [HttpGet, Authorize(Policy = Permissions.GovernmentEntityView)] public async Task<ActionResult<PagedResult<GovernmentEntityResponse>>> GetAll([FromQuery] GovernmentEntityListQuery q, CancellationToken ct) => Ok(await service.GetAllAsync(q, ct));
    [HttpGet("eliminadas"), Authorize(Policy = Permissions.GovernmentEntityRestore)] public async Task<ActionResult<PagedResult<GovernmentEntityResponse>>> GetDeleted([FromQuery] GovernmentEntityListQuery q, CancellationToken ct) => Ok(await service.GetDeletedAsync(q, ct));
    [HttpGet("{id:guid}"), Authorize(Policy = Permissions.GovernmentEntityView)] public async Task<ActionResult<GovernmentEntityResponse>> GetById(Guid id, CancellationToken ct) => Ok(await service.GetByIdAsync(id, ct));
    [HttpPost, Authorize(Policy = Permissions.GovernmentEntityCreate)] public async Task<ActionResult<GovernmentEntityResponse>> Create(CreateGovernmentEntityRequest r, CancellationToken ct) { var entity = await service.CreateAsync(r, ct); return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity); }
    [HttpPut("{id:guid}"), Authorize(Policy = Permissions.GovernmentEntityUpdate)] public async Task<ActionResult<GovernmentEntityResponse>> Update(Guid id, UpdateGovernmentEntityRequest r, CancellationToken ct) => Ok(await service.UpdateAsync(id, r, ct));
    [HttpPatch("{id:guid}/estado"), Authorize(Policy = Permissions.GovernmentEntityUpdate)] public async Task<IActionResult> Status(Guid id, ChangeGovernmentEntityStatusRequest r, CancellationToken ct) { await service.ChangeStatusAsync(id, r, ct); return NoContent(); }
    [HttpDelete("{id:guid}"), Authorize(Policy = Permissions.GovernmentEntityDelete)] public async Task<IActionResult> Delete(Guid id, [FromQuery] string rowVersion, CancellationToken ct) { await service.DeleteAsync(id, rowVersion, ct); return NoContent(); }
    [HttpPatch("{id:guid}/restaurar"), Authorize(Policy = Permissions.GovernmentEntityRestore)] public async Task<IActionResult> Restore(Guid id, [FromQuery] string rowVersion, CancellationToken ct) { await service.RestoreAsync(id, rowVersion, ct); return NoContent(); }
}
