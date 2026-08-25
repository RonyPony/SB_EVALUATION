namespace SB.BACKEND.Application.Security;

public static class Permissions
{
    public const string ClaimType = "permission";
    public const string UserView = "SECURITY.USER.VIEW";
    public const string UserCreate = "SECURITY.USER.CREATE";
    public const string UserUpdate = "SECURITY.USER.UPDATE";
    public const string UserDelete = "SECURITY.USER.DELETE";
    public const string UserAssignRole = "SECURITY.USER.ASSIGN_ROLE";
    public const string RoleView = "SECURITY.ROLE.VIEW";
    public const string RoleCreate = "SECURITY.ROLE.CREATE";
    public const string RoleUpdate = "SECURITY.ROLE.UPDATE";
    public const string RoleDelete = "SECURITY.ROLE.DELETE";
    public const string RoleAssignPermission = "SECURITY.ROLE.ASSIGN_PERMISSION";
    public const string PermissionView = "SECURITY.PERMISSION.VIEW";

    public static readonly IReadOnlyCollection<string> All =
    [ UserView, UserCreate, UserUpdate, UserDelete, UserAssignRole, RoleView, RoleCreate,
      RoleUpdate, RoleDelete, RoleAssignPermission, PermissionView ];
}
