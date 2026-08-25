using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.BACKEND.Application.Authentication;
namespace SB.BACKEND.Api.Controllers;
[ApiController, Route("api/[controller]")]
public sealed class AuthController(IUserCredentialValidator validator, IJwtTokenService tokens) : ControllerBase
{
    [AllowAnonymous, HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        var user = validator.Validate(request.Username, request.Password);
        if (user is null)
            return Problem(statusCode: 401, title: "Invalid credentials",
                detail: "The supplied username or password is invalid.", type: "https://httpstatuses.com/401");
        var token = tokens.GenerateToken(user);
        return Ok(new LoginResponse(token.AccessToken, "Bearer", token.ExpiresAt));
    }
}
