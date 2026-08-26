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
    public const string GovernmentEntityView = "GOVERNMENT_ENTITY.VIEW";
    public const string GovernmentEntityCreate = "GOVERNMENT_ENTITY.CREATE";
    public const string GovernmentEntityUpdate = "GOVERNMENT_ENTITY.UPDATE";
    public const string GovernmentEntityDelete = "GOVERNMENT_ENTITY.DELETE";
    public const string GovernmentEntityRestore = "GOVERNMENT_ENTITY.RESTORE";

    public static readonly IReadOnlyCollection<string> All =
    [ UserView, UserCreate, UserUpdate, UserDelete, UserAssignRole, RoleView, RoleCreate,
      RoleUpdate, RoleDelete, RoleAssignPermission, PermissionView, GovernmentEntityView, GovernmentEntityCreate,
      GovernmentEntityUpdate, GovernmentEntityDelete, GovernmentEntityRestore ];
}
