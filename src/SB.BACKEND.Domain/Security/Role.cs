using SB.BACKEND.Domain.Common;

namespace SB.BACKEND.Domain.Security;

public sealed class Role : BaseEntity
{
    private Role() { }
    public Role(string name, string normalizedName, string? description = null)
    { Name = name; NormalizedName = normalizedName; Description = description; }

    public string Name { get; private set; } = string.Empty;
    public string NormalizedName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    public void Update(string name, string normalizedName, string? description)
    { Name = name; NormalizedName = normalizedName; Description = description; MarkAsUpdated(); }
}
