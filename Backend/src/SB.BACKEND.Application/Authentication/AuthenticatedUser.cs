namespace SB.BACKEND.Application.Authentication;

public sealed record AuthenticatedUser(
    Guid Id,
    string Username,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions
);
