using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace SB.BACKEND.Api.Controllers;
[ApiController, Route("api/[controller]")]
public sealed class SampleController : ControllerBase
{
    [AllowAnonymous, HttpGet("public")]
    public IActionResult Public() => Ok(new { message = "This endpoint is public." });
    [Authorize, HttpGet("protected")]
    public IActionResult Protected() => Ok(new { message = "This endpoint is protected.", username = User.Identity?.Name,
        roles = User.FindAll(ClaimTypes.Role).Select(claim => claim.Value) });
    [Authorize(Roles = "Admin"), HttpGet("admin")]
    public IActionResult Admin() => Ok(new { message = "The Admin role was validated." });
}
