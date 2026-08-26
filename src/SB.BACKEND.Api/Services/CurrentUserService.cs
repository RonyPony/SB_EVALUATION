using System.Security.Claims;
using SB.BACKEND.Application.Common;
namespace SB.BACKEND.Api.Services;
internal sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{ public Guid? UserId => Guid.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null; }
