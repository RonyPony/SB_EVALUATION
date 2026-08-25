namespace SB.BACKEND.Application.Authentication;
public sealed record AuthenticatedUser(string Username, IReadOnlyCollection<string> Roles, IReadOnlyDictionary<string, string> Claims);
