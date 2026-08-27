using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.BACKEND.Application.Security;

namespace SB.BACKEND.Api.Controllers;

[ApiController, Authorize, Route("api/[controller]")]
public sealed class RolesController(IRoleService roles) : ControllerBase
{
    [HttpGet, Authorize(Policy = Permissions.ROLE_VIEW)]
    public async Task<ActionResult<IReadOnlyCollection<RoleResponse>>> GetAll(CancellationToken ct)
    {
        return Ok(await roles.GetAllAsync(ct));
    }

    [HttpPost, Authorize(Policy = Permissions.ROLE_CREATE)]
    public async Task<ActionResult<RoleResponse>> Create(
        CreateRoleRequest request,
        CancellationToken ct
    )
    {
        var role = await roles.CreateAsync(request, ct);
        return Created($"/api/roles/{role.Id}", role);
    }

    [HttpPut("{id:guid}"), Authorize(Policy = Permissions.ROLE_UPDATE)]
    public async Task<ActionResult<RoleResponse>> Update(
        Guid id,
        UpdateRoleRequest request,
        CancellationToken ct
    )
    {
        return Ok(await roles.UpdateAsync(id, request, ct));
    }

    [HttpDelete("{id:guid}"), Authorize(Policy = Permissions.ROLE_DELETE)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await roles.DeleteAsync(id, ct);
        return NoContent();
    }

    [
        HttpPut("{roleId:guid}/permissions/{permissionId:guid}"),
        Authorize(Policy = Permissions.ROLE_ASSIGN_PERMISSION)
    ]
    public async Task<IActionResult> AssignPermission(
        Guid roleId,
        Guid permissionId,
        CancellationToken ct
    )
    {
        await roles.AssignPermissionAsync(roleId, permissionId, ct);
        return NoContent();
    }

    [
        HttpDelete("{roleId:guid}/permissions/{permissionId:guid}"),
        Authorize(Policy = Permissions.ROLE_ASSIGN_PERMISSION)
    ]
    public async Task<IActionResult> RemovePermission(
        Guid roleId,
        Guid permissionId,
        CancellationToken ct
    )
    {
        await roles.RemovePermissionAsync(roleId, permissionId, ct);
        return NoContent();
    }
}
