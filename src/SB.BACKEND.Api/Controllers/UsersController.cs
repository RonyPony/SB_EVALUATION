using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.BACKEND.Application.Security;

namespace SB.BACKEND.Api.Controllers;

[ApiController, Authorize, Route("api/[controller]")]
public sealed class UsersController(IUserService users) : ControllerBase
{
    [HttpGet, Authorize(Policy = Permissions.UserView)]
    public async Task<ActionResult<IReadOnlyCollection<UserResponse>>> GetAll(CancellationToken ct) => Ok(await users.GetAllAsync(ct));
    [HttpGet("{id:guid}"), Authorize(Policy = Permissions.UserView)]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct) => Ok(await users.GetByIdAsync(id, ct));
    [HttpPut("{userId:guid}/roles/{roleId:guid}"), Authorize(Policy = Permissions.UserAssignRole)]
    public async Task<IActionResult> AssignRole(Guid userId, Guid roleId, CancellationToken ct)
    { await users.AssignRoleAsync(userId, roleId, ct); return NoContent(); }
    [HttpDelete("{userId:guid}/roles/{roleId:guid}"), Authorize(Policy = Permissions.UserAssignRole)]
    public async Task<IActionResult> RemoveRole(Guid userId, Guid roleId, CancellationToken ct)
    { await users.RemoveRoleAsync(userId, roleId, ct); return NoContent(); }
}
