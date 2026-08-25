namespace SB.BACKEND.Application.Authentication;
public interface IJwtTokenService { TokenResult GenerateToken(AuthenticatedUser user); }
