namespace SB.BACKEND.Application.Common;
public interface ICurrentUserService
{
    Guid? UserId { get; }
    IReadOnlyCollection<string> Roles { get; }
    bool IsInRole(params string[] roles);
}
