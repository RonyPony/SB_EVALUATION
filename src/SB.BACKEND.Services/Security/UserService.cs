using SB.BACKEND.Application.Common;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Domain.Security;

namespace SB.BACKEND.Services.Security;

internal sealed class UserService(IUserRepository users, IRoleRepository roles, IUnitOfWork unitOfWork) : IUserService
{
    public async Task<IReadOnlyCollection<UserResponse>> GetAllAsync(CancellationToken ct) => (await users.GetAllAsync(ct)).Select(x => x.ToResponse()).ToArray();
    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken ct) => (await GetUserAsync(id, ct)).ToResponse();
    public async Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
    {
        var user = await GetUserAsync(userId, ct); _ = await GetRoleAsync(roleId, ct);
        if (user.UserRoles.Any(x => x.RoleId == roleId)) throw new ConflictException("The user already has this role.");
        user.UserRoles.Add(new UserRole(userId, roleId)); await unitOfWork.SaveChangesAsync(ct);
    }
    public async Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
    {
        var user = await GetUserAsync(userId, ct);
        var relation = user.UserRoles.SingleOrDefault(x => x.RoleId == roleId) ?? throw new NotFoundException("The user does not have this role.");
        user.UserRoles.Remove(relation); await unitOfWork.SaveChangesAsync(ct);
    }
    private async Task<User> GetUserAsync(Guid id, CancellationToken ct) => await users.GetByIdAsync(id, ct) ?? throw new NotFoundException("User not found.");
    private async Task<Role> GetRoleAsync(Guid id, CancellationToken ct) => await roles.GetByIdAsync(id, ct) ?? throw new NotFoundException("Role not found.");
}
