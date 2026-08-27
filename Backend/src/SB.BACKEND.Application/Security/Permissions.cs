namespace SB.BACKEND.Application.Security;

public static class Permissions
{
    public const string CLAIM_TYPE = "permission";
    public const string USER_VIEW = "SECURITY.USER.VIEW";
    public const string USER_CREATE = "SECURITY.USER.CREATE";
    public const string USER_UPDATE = "SECURITY.USER.UPDATE";
    public const string USER_DELETE = "SECURITY.USER.DELETE";
    public const string USER_ASSIGN_ROLE = "SECURITY.USER.ASSIGN_ROLE";
    public const string ROLE_VIEW = "SECURITY.ROLE.VIEW";
    public const string ROLE_CREATE = "SECURITY.ROLE.CREATE";
    public const string ROLE_UPDATE = "SECURITY.ROLE.UPDATE";
    public const string ROLE_DELETE = "SECURITY.ROLE.DELETE";
    public const string ROLE_ASSIGN_PERMISSION = "SECURITY.ROLE.ASSIGN_PERMISSION";
    public const string PERMISSION_VIEW = "SECURITY.PERMISSION.VIEW";
    public const string GOVERNMENT_ENTITY_VIEW = "GOVERNMENT_ENTITY.VIEW";
    public const string GOVERNMENT_ENTITY_CREATE = "GOVERNMENT_ENTITY.CREATE";
    public const string GOVERNMENT_ENTITY_UPDATE = "GOVERNMENT_ENTITY.UPDATE";
    public const string GOVERNMENT_ENTITY_DELETE = "GOVERNMENT_ENTITY.DELETE";
    public const string GOVERNMENT_ENTITY_RESTORE = "GOVERNMENT_ENTITY.RESTORE";

    public static readonly IReadOnlyCollection<string> All =
    [
        USER_VIEW,
        USER_CREATE,
        USER_UPDATE,
        USER_DELETE,
        USER_ASSIGN_ROLE,
        ROLE_VIEW,
        ROLE_CREATE,
        ROLE_UPDATE,
        ROLE_DELETE,
        ROLE_ASSIGN_PERMISSION,
        PERMISSION_VIEW,
        GOVERNMENT_ENTITY_VIEW,
        GOVERNMENT_ENTITY_CREATE,
        GOVERNMENT_ENTITY_UPDATE,
        GOVERNMENT_ENTITY_DELETE,
        GOVERNMENT_ENTITY_RESTORE,
    ];
}
