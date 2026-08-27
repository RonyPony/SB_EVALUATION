using SB.BACKEND.Domain.Common;

namespace SB.BACKEND.Domain.Security;

public sealed class User : BaseEntity
{
    private User() { }

    public User(
        string username,
        string normalizedUsername,
        string email,
        string normalizedEmail,
        string passwordHash
    )
    {
        Username = username;
        NormalizedUsername = normalizedUsername;
        Email = email;
        NormalizedEmail = normalizedEmail;
        PasswordHash = passwordHash;
    }

    public string Username { get; private set; } = string.Empty;
    public string NormalizedUsername { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string NormalizedEmail { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public ICollection<UserRole> UserRoles { get; private set; } = [];

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        MarkAsUpdated();
    }
}
