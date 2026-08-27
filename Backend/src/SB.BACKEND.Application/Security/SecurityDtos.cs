using System.ComponentModel.DataAnnotations;

namespace SB.BACKEND.Application.Security;

public sealed class RegisterUserRequest
{
    [Required, StringLength(50, MinimumLength = 3)]
    public string Username { get; init; } = string.Empty;

    [Required, EmailAddress, StringLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;
}

public sealed class CreateRoleRequest
{
    [Required, StringLength(50, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; init; }
}

public sealed class UpdateRoleRequest
{
    [Required, StringLength(50, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; init; }
}

public sealed record UserResponse(
    Guid Id,
    string Username,
    string Email,
    bool IsActive,
    DateTimeOffset CreatedAt,
    IReadOnlyCollection<string> Roles
);

public sealed record RoleResponse(
    Guid Id,
    string Name,
    string? Description,
    IReadOnlyCollection<string> Permissions
);

public sealed record PermissionResponse(Guid Id, string Name, string Description);
