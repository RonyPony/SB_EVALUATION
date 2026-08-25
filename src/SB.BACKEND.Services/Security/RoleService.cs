using SB.BACKEND.Application.Common;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Domain.Security;

namespace SB.BACKEND.Services.Security;

internal sealed class RoleService(IRoleRepository roles, IPermissionRepository permissions, IUnitOfWork unitOfWork) : IRoleService
{
    public async Task<IReadOnlyCollection<RoleResponse>> GetAllAsync(CancellationToken ct) => (await roles.GetAllAsync(ct)).Select(x => x.ToResponse()).ToArray();
    public async Task<RoleResponse> CreateAsync(CreateRoleRequest request, CancellationToken ct)
    {
        var name = request.Name.Trim(); var normalized = SecurityMappings.Normalize(name);
        if (await roles.NameExistsAsync(normalized, null, ct)) throw new ConflictException("The role name already exists.");
        var role = new Role(name, normalized, request.Description?.Trim()); roles.Add(role); await unitOfWork.SaveChangesAsync(ct); return role.ToResponse();
    }
    public async Task<RoleResponse> UpdateAsync(Guid id, UpdateRoleRequest request, CancellationToken ct)
    {
        var role = await GetRoleAsync(id, ct); var name = request.Name.Trim(); var normalized = SecurityMappings.Normalize(name);
        if (await roles.NameExistsAsync(normalized, id, ct)) throw new ConflictException("The role name already exists.");
        role.Update(name, normalized, request.Description?.Trim()); await unitOfWork.SaveChangesAsync(ct); return role.ToResponse();
    }
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    { var role = await GetRoleAsync(id, ct); roles.Remove(role); await unitOfWork.SaveChangesAsync(ct); }
    public async Task AssignPermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct)
    {
        var role = await GetRoleAsync(roleId, ct); _ = await GetPermissionAsync(permissionId, ct);
        if (role.RolePermissions.Any(x => x.PermissionId == permissionId)) throw new ConflictException("The role already has this permission.");
        role.RolePermissions.Add(new RolePermission(roleId, permissionId)); await unitOfWork.SaveChangesAsync(ct);
    }
    public async Task RemovePermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct)
    {
        var role = await GetRoleAsync(roleId, ct);
        var relation = role.RolePermissions.SingleOrDefault(x => x.PermissionId == permissionId) ?? throw new NotFoundException("The role does not have this permission.");
        role.RolePermissions.Remove(relation); await unitOfWork.SaveChangesAsync(ct);
    }
    private async Task<Role> GetRoleAsync(Guid id, CancellationToken ct) => await roles.GetByIdAsync(id, ct) ?? throw new NotFoundException("Role not found.");
    private async Task<Permission> GetPermissionAsync(Guid id, CancellationToken ct) => await permissions.GetByIdAsync(id, ct) ?? throw new NotFoundException("Permission not found.");
}
