using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.BACKEND.Application.Authentication;
using SB.BACKEND.Application.Common;
using SB.BACKEND.Application.Security;
namespace SB.BACKEND.Api.Controllers;
[ApiController, Route("api/[controller]")]
public sealed class AuthController(IAuthenticationService authentication, IUserService users, ICurrentUserService currentUser) : ControllerBase
{
    [AllowAnonymous, HttpPost("register")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserResponse>> Register(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var user = await authentication.RegisterAsync(request, cancellationToken);
        return CreatedAtAction(nameof(UsersController.GetById), "Users", new { id = user.Id }, user);
    }

    [AllowAnonymous, HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await authentication.LoginAsync(request, cancellationToken);
        if (response is null)
            return Problem(statusCode: 401, title: "Invalid credentials",
                detail: "The supplied username or password is invalid.", type: "https://httpstatuses.com/401");
        return Ok(response);
    }

    [Authorize, HttpGet("me")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserResponse>> Me(CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid id) return Unauthorized();
        return Ok(await users.GetByIdAsync(id, cancellationToken));
    }
}
