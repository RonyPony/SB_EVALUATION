using SB.BACKEND.Application.Common;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Domain.Security;

namespace SB.BACKEND.Services.Security;

internal sealed class UserService(
    IUserRepository users,
    IRoleRepository roles,
    IUnitOfWork unitOfWork
) : IUserService
{
    public async Task<IReadOnlyCollection<UserResponse>> GetAllAsync(CancellationToken ct)
    {
        return
        [
            .. (await users.GetAllAsync(ct)).Select(x =>
            {
                return x.ToResponse();
            }),
        ];
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return (await GetUserAsync(id, ct)).ToResponse();
    }

    public async Task<PagedResult<UserResponse>> GetAnalystsAsync(
        string? search,
        int pageNumber,
        int pageSize,
        CancellationToken ct
    )
    {
        if (pageNumber < 1 || pageSize is < 1 or > 100)
            throw new ValidationException("La paginación no es válida.");
        var source = (await users.GetAllAsync(ct)).Where(x =>
        {
            return x.IsActive
                && x.UserRoles.Any(r =>
                {
                    return r.Role.Name is "Analista" or "Administrador" or "Admin";
                });
        });
        if (!string.IsNullOrWhiteSpace(search))
            source = source.Where(x =>
            {
                return x.Username.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase)
                    || x.Email.Contains(search.Trim(), StringComparison.OrdinalIgnoreCase);
            });
        var values = source
            .OrderBy(x =>
            {
                return x.Username;
            })
            .ToArray();
        return new(
            [
                .. values
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x =>
                    {
                        return x.ToResponse();
                    }),
            ],
            pageNumber,
            pageSize,
            values.Length
        );
    }

    public async Task AssignRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
    {
        var user = await GetUserAsync(userId, ct);
        _ = await GetRoleAsync(roleId, ct);
        if (
            user.UserRoles.Any(x =>
            {
                return x.RoleId == roleId;
            })
        )
            throw new ConflictException("The user already has this role.");
        user.UserRoles.Add(new UserRole(userId, roleId));
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RemoveRoleAsync(Guid userId, Guid roleId, CancellationToken ct)
    {
        var user = await GetUserAsync(userId, ct);
        var relation =
            user.UserRoles.SingleOrDefault(x =>
            {
                return x.RoleId == roleId;
            }) ?? throw new NotFoundException("The user does not have this role.");
        user.UserRoles.Remove(relation);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<User> GetUserAsync(Guid id, CancellationToken ct)
    {
        return await users.GetByIdAsync(id, ct) ?? throw new NotFoundException("User not found.");
    }

    private async Task<Role> GetRoleAsync(Guid id, CancellationToken ct)
    {
        return await roles.GetByIdAsync(id, ct) ?? throw new NotFoundException("Role not found.");
    }
}
