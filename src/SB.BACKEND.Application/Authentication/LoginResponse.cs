namespace SB.BACKEND.Application.Authentication;
public sealed record LoginResponse(string AccessToken, string TokenType, DateTimeOffset ExpiresAt);
