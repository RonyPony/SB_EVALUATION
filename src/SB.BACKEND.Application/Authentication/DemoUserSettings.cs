namespace SB.BACKEND.Application.Authentication;
public sealed class DemoUserSettings
{
    public const string SectionName = "DemoUser";
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string[] Roles { get; init; } = [];
    public Dictionary<string, string> Claims { get; init; } = [];
}
