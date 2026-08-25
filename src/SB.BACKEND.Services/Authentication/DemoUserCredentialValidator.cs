using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using SB.BACKEND.Application.Authentication;
namespace SB.BACKEND.Services.Authentication;
internal sealed class DemoUserCredentialValidator(IOptions<DemoUserSettings> options) : IUserCredentialValidator
{
    private readonly DemoUserSettings _user = options.Value;
    public AuthenticatedUser? Validate(string username, string password)
    {
        var valid = FixedTimeEquals(username, _user.Username) && FixedTimeEquals(password, _user.Password);
        return valid ? new AuthenticatedUser(_user.Username, _user.Roles, _user.Claims) : null;
    }
    private static bool FixedTimeEquals(string supplied, string expected)
    {
        var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(supplied));
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(expected));
        return CryptographicOperations.FixedTimeEquals(suppliedHash, expectedHash);
    }
}
