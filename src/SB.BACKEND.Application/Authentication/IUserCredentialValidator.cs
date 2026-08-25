namespace SB.BACKEND.Application.Authentication;
public interface IUserCredentialValidator { AuthenticatedUser? Validate(string username, string password); }
