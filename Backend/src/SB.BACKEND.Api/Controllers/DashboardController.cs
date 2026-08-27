using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.BACKEND.Application.Support;

namespace SB.BACKEND.Api.Controllers;

[ApiController, Route("api/dashboard"), Authorize]
public sealed class DashboardController(IDashboardService service) : ControllerBase
{
    [HttpGet, HttpGet("solicitudes")]
    public Task<DashboardResponse> Get(CancellationToken ct)
    {
        return service.GetAsync(ct);
    }
}
