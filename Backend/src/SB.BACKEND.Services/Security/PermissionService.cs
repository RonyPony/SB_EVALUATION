using SB.BACKEND.Application.Security;

namespace SB.BACKEND.Services.Security;

internal sealed class PermissionService(IPermissionRepository permissions) : IPermissionService
{
    public async Task<IReadOnlyCollection<PermissionResponse>> GetAllAsync(CancellationToken ct)
    {
        return
        [
            .. (await permissions.GetAllAsync(ct)).Select(x =>
            {
                return x.ToResponse();
            }),
        ];
    }
}
