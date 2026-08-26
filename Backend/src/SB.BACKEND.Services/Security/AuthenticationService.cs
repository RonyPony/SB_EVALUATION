using SB.BACKEND.Application.Authentication;
using SB.BACKEND.Application.Common;
using SB.BACKEND.Application.Security;
using SB.BACKEND.Domain.Security;

namespace SB.BACKEND.Services.Security;

internal sealed class AuthenticationService(IUserRepository users, IRoleRepository roles, IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher, IJwtTokenService tokens) : IAuthenticationService
{
    public async Task<UserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken ct)
    {
        var username = request.Username.Trim(); var email = request.Email.Trim();
        var normalizedUsername = SecurityMappings.Normalize(username); var normalizedEmail = SecurityMappings.Normalize(email);
        if (await users.UsernameExistsAsync(normalizedUsername, ct)) throw new ConflictException("The username is already registered.");
        if (await users.EmailExistsAsync(normalizedEmail, ct)) throw new ConflictException("The email address is already registered.");

        var user = new User(username, normalizedUsername, email, normalizedEmail, passwordHasher.Hash(request.Password));
        var defaultRole = await roles.GetByNameAsync("USER", ct);
        if (defaultRole is not null) user.UserRoles.Add(new UserRole(user.Id, defaultRole.Id));
        users.Add(user); await unitOfWork.SaveChangesAsync(ct);
        return user.ToResponse();
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await users.GetByUsernameAsync(SecurityMappings.Normalize(request.Username), ct);
        if (user is null || !user.IsActive || !passwordHasher.Verify(request.Password, user.PasswordHash)) return null;
        var roles = user.UserRoles.Select(x => x.Role.Name).Distinct().ToArray();
        var permissions = user.UserRoles.SelectMany(x => x.Role.RolePermissions).Select(x => x.Permission.Name).Distinct().ToArray();
        var token = tokens.GenerateToken(new AuthenticatedUser(user.Id, user.Username, roles, permissions));
        return new LoginResponse(token.AccessToken, "Bearer", token.ExpiresAt);
    }
}
