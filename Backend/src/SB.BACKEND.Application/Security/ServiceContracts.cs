using SB.BACKEND.Application.Authentication;
using SB.BACKEND.Application.Common;

namespace SB.BACKEND.Application.Security;

public interface IAuthenticationService
{
    Task<UserResponse> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken
    );
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}

public interface IUserService
{
    Task<IReadOnlyCollection<UserResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<UserResponse>> GetAnalystsAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken
    );
    Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
    Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);
}

public interface IRoleService
{
    Task<IReadOnlyCollection<RoleResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<RoleResponse> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken);
    Task<RoleResponse> UpdateAsync(
        Guid id,
        UpdateRoleRequest request,
        CancellationToken cancellationToken
    );
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task AssignPermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken);
    Task RemovePermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken);
}

public interface IPermissionService
{
    Task<IReadOnlyCollection<PermissionResponse>> GetAllAsync(CancellationToken cancellationToken);
}
