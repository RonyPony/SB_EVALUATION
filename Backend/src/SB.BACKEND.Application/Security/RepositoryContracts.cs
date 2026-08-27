using SB.BACKEND.Domain.Security;

namespace SB.BACKEND.Application.Security;

public interface IUserRepository
{
    Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<User?> GetByUsernameAsync(string normalizedUsername, CancellationToken cancellationToken);
    Task<bool> UsernameExistsAsync(string normalizedUsername, CancellationToken cancellationToken);
    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken);
    void Add(User user);
}

public interface IRoleRepository
{
    Task<IReadOnlyCollection<Role>> GetAllAsync(CancellationToken cancellationToken);
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Role?> GetByNameAsync(string normalizedName, CancellationToken cancellationToken);
    Task<bool> NameExistsAsync(
        string normalizedName,
        Guid? excludingId,
        CancellationToken cancellationToken
    );
    void Add(Role role);
    void Remove(Role role);
}

public interface IPermissionRepository
{
    Task<IReadOnlyCollection<Permission>> GetAllAsync(CancellationToken cancellationToken);
    Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}
