using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.BACKEND.Application.Security;

namespace SB.BACKEND.Api.Controllers;

[ApiController, Authorize, Route("api/[controller]")]
public sealed class RolesController(IRoleService roles) : ControllerBase
{
    [HttpGet, Authorize(Policy = Permissions.RoleView)]
    public async Task<ActionResult<IReadOnlyCollection<RoleResponse>>> GetAll(CancellationToken ct) => Ok(await roles.GetAllAsync(ct));
    [HttpPost, Authorize(Policy = Permissions.RoleCreate)]
    public async Task<ActionResult<RoleResponse>> Create(CreateRoleRequest request, CancellationToken ct)
    { var role = await roles.CreateAsync(request, ct); return Created($"/api/roles/{role.Id}", role); }
    [HttpPut("{id:guid}"), Authorize(Policy = Permissions.RoleUpdate)]
    public async Task<ActionResult<RoleResponse>> Update(Guid id, UpdateRoleRequest request, CancellationToken ct) => Ok(await roles.UpdateAsync(id, request, ct));
    [HttpDelete("{id:guid}"), Authorize(Policy = Permissions.RoleDelete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    { await roles.DeleteAsync(id, ct); return NoContent(); }
    [HttpPut("{roleId:guid}/permissions/{permissionId:guid}"), Authorize(Policy = Permissions.RoleAssignPermission)]
    public async Task<IActionResult> AssignPermission(Guid roleId, Guid permissionId, CancellationToken ct)
    { await roles.AssignPermissionAsync(roleId, permissionId, ct); return NoContent(); }
    [HttpDelete("{roleId:guid}/permissions/{permissionId:guid}"), Authorize(Policy = Permissions.RoleAssignPermission)]
    public async Task<IActionResult> RemovePermission(Guid roleId, Guid permissionId, CancellationToken ct)
    { await roles.RemovePermissionAsync(roleId, permissionId, ct); return NoContent(); }
}
