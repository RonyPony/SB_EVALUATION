using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SB.BACKEND.Application.Authentication;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Domain.Security;

namespace SB.BACKEND.Infrastructure.Persistence;

public interface IDatabaseInitializer { Task InitializeAsync(CancellationToken cancellationToken = default); }

internal sealed class DatabaseInitializer(SecurityDbContext dbContext, IPasswordHasher passwordHasher,
    IOptions<DemoUserSettings> demoOptions, IGovernmentEntitySeeder governmentSeeder) : IDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await dbContext.Database.MigrateAsync(ct);
        var demo = demoOptions.Value;
        var admin = await GetOrCreateRoleAsync("Admin", "Full security administration role.", ct);
        var userRole = await GetOrCreateRoleAsync("User", "Default application user role.", ct);
        var permissions = await dbContext.Permissions.ToListAsync(ct);
        foreach (var permission in permissions)
            if (admin.RolePermissions.All(x => x.PermissionId != permission.Id)) admin.RolePermissions.Add(new RolePermission(admin.Id, permission.Id));

        if (!string.IsNullOrWhiteSpace(demo.Username) && !string.IsNullOrWhiteSpace(demo.Password))
        {
            var normalizedUsername = demo.Username.Trim().ToUpperInvariant();
            if (!await dbContext.Users.AnyAsync(x => x.NormalizedUsername == normalizedUsername, ct))
            {
                var email = string.IsNullOrWhiteSpace(demo.Email) ? $"{demo.Username}@local.invalid" : demo.Email.Trim();
                var user = new User(demo.Username.Trim(), normalizedUsername, email, email.ToUpperInvariant(), passwordHasher.Hash(demo.Password));
                user.UserRoles.Add(new UserRole(user.Id, admin.Id)); user.UserRoles.Add(new UserRole(user.Id, userRole.Id)); dbContext.Users.Add(user);
            }
        }
        await dbContext.SaveChangesAsync(ct);
        await governmentSeeder.SeedAsync(ct);
    }

    private async Task<Role> GetOrCreateRoleAsync(string name, string description, CancellationToken ct)
    {
        var normalized = name.ToUpperInvariant();
        var role = await dbContext.Roles.Include(x => x.RolePermissions).SingleOrDefaultAsync(x => x.NormalizedName == normalized, ct);
        if (role is not null) return role;
        role = new Role(name, normalized, description); dbContext.Roles.Add(role); return role;
    }
}
