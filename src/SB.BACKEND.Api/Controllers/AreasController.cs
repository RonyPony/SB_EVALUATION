using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.BACKEND.Application.Support;
namespace SB.BACKEND.Api.Controllers;
[ApiController, Route("api/areas"), Authorize]
public sealed class AreasController(IAreaService service):ControllerBase
{
    [HttpGet] public Task<IReadOnlyCollection<AreaResponse>> GetAll([FromQuery]bool activeOnly=true,CancellationToken ct=default)=>service.GetAllAsync(activeOnly,ct);
    [HttpGet("{id:guid}")] public Task<AreaResponse> GetById(Guid id,CancellationToken ct)=>service.GetByIdAsync(id,ct);
    [HttpPost,Authorize(Policy="SupportAdmin")] public async Task<ActionResult<AreaResponse>> Create(CreateAreaRequest request,CancellationToken ct){var result=await service.CreateAsync(request,ct);return CreatedAtAction(nameof(GetById),new{id=result.Id},result);}
    [HttpPut("{id:guid}"),Authorize(Policy="SupportAdmin")] public Task<AreaResponse> Update(Guid id,UpdateAreaRequest request,CancellationToken ct)=>service.UpdateAsync(id,request,ct);
    [HttpPatch("{id:guid}/estado"),Authorize(Policy="SupportAdmin")] public async Task<IActionResult> ChangeStatus(Guid id,ChangeAreaStatusRequest request,CancellationToken ct){await service.ChangeStatusAsync(id,request,ct);return NoContent();}
    [HttpDelete("{id:guid}"),Authorize(Policy="SupportAdmin")] public async Task<IActionResult> Delete(Guid id,[FromQuery]string rowVersion,CancellationToken ct){await service.DeleteAsync(id,rowVersion,ct);return NoContent();}
}
