namespace SB.BACKEND.Application.Authentication;
public sealed record TokenResult(string AccessToken, DateTimeOffset ExpiresAt);
