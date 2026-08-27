using System.Security.Claims;
using SB.BACKEND.Application.Common;

namespace SB.BACKEND.Api.Services;

internal sealed class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            return Guid.TryParse(
                accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier),
                out var id
            )
                ? id
                : null;
        }
    }

    public IReadOnlyCollection<string> Roles
    {
        get
        {
            return accessor
                    .HttpContext?.User.FindAll(ClaimTypes.Role)
                    .Select(x =>
                    {
                        return x.Value;
                    })
                    .ToArray()
                ?? [];
        }
    }

    public bool IsInRole(params string[] roles)
    {
        return roles.Any(role =>
        {
            return accessor.HttpContext?.User.IsInRole(role) == true;
        });
    }
}
