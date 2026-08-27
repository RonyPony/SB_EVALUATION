using SB.BACKEND.Application.Security;
using SB.BACKEND.Domain.Security;

namespace SB.BACKEND.Services.Security;

internal static class SecurityMappings
{
    public static UserResponse ToResponse(this User user)
    {
        return new(
            user.Id,
            user.Username,
            user.Email,
            user.IsActive,
            user.CreatedAt,
            [
                .. user
                    .UserRoles.Select(x =>
                    {
                        return x.Role.Name;
                    })
                    .OrderBy(x =>
                    {
                        return x;
                    }),
            ]
        );
    }

    public static RoleResponse ToResponse(this Role role)
    {
        return new(
            role.Id,
            role.Name,
            role.Description,
            [
                .. role
                    .RolePermissions.Select(x =>
                    {
                        return x.Permission.Name;
                    })
                    .OrderBy(x =>
                    {
                        return x;
                    }),
            ]
        );
    }

    public static PermissionResponse ToResponse(this Permission permission)
    {
        return new(permission.Id, permission.Name, permission.Description);
    }

    public static string Normalize(string value)
    {
        return value.Trim().ToUpperInvariant();
    }
}
