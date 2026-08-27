using System.Security.Cryptography;
using SB.BACKEND.Application.Security;

namespace SB.BACKEND.Services.Security;

internal sealed class PasswordHasher : IPasswordHasher
{
    private const int ITERATIONS = 210_000;
    private const int SALT_SIZE = 16;
    private const int HASH_SIZE = 32;

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        var salt = RandomNumberGenerator.GetBytes(SALT_SIZE);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            ITERATIONS,
            HashAlgorithmName.SHA256,
            HASH_SIZE
        );
        return $"PBKDF2-SHA256${ITERATIONS}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string passwordHash)
    {
        try
        {
            var parts = passwordHash.Split('$');
            if (parts.Length != 4 || parts[0] != "PBKDF2-SHA256")
                return false;
            var iterations = int.Parse(parts[1]);
            var salt = Convert.FromBase64String(parts[2]);
            var expected = Convert.FromBase64String(parts[3]);
            var actual = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expected.Length
            );
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
