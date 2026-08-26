using System.ComponentModel.DataAnnotations;
namespace SB.BACKEND.Application.Authentication;
public sealed class LoginRequest
{
    [Required]
    public string Username { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
