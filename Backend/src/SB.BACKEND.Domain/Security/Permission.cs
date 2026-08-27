using SB.BACKEND.Domain.Common;

namespace SB.BACKEND.Domain.Security;

public sealed class Permission : BaseEntity
{
    private Permission() { }

    public Permission(string name, string description)
    {
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
    }

    public string Name { get; private set; } = string.Empty;
    public string NormalizedName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public ICollection<RolePermission> RolePermissions { get; private set; } = [];
}
