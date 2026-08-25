using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.BACKEND.Application.Security;

namespace SB.BACKEND.Api.Controllers;

[ApiController, Authorize(Policy = Permissions.PermissionView), Route("api/[controller]")]
public sealed class PermissionsController(IPermissionService permissions) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PermissionResponse>>> GetAll(CancellationToken ct) => Ok(await permissions.GetAllAsync(ct));
}
