namespace SB.BACKEND.Application.Authentication;

public sealed class DemoUserSettings
{
    public const string SECTION_NAME = "DemoUser";
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
