using SB.BACKEND.Application.Common;
using SB.BACKEND.Domain.GovernmentEntities;
namespace SB.BACKEND.Application.GovernmentEntities;
public interface IGovernmentEntityService
{
    Task<PagedResult<GovernmentEntityResponse>> GetAllAsync(GovernmentEntityListQuery query, CancellationToken ct);
    Task<PagedResult<GovernmentEntityResponse>> GetDeletedAsync(GovernmentEntityListQuery query, CancellationToken ct);
    Task<GovernmentEntityResponse> GetByIdAsync(Guid id, CancellationToken ct);
    Task<GovernmentEntityResponse> CreateAsync(CreateGovernmentEntityRequest request, CancellationToken ct);
    Task<GovernmentEntityResponse> UpdateAsync(Guid id, UpdateGovernmentEntityRequest request, CancellationToken ct);
    Task ChangeStatusAsync(Guid id, ChangeGovernmentEntityStatusRequest request, CancellationToken ct);
    Task DeleteAsync(Guid id, string version, CancellationToken ct); Task RestoreAsync(Guid id, string version, CancellationToken ct);
}
public interface IGovernmentEntityRepository
{
    Task<(IReadOnlyCollection<EntidadGubernamental> Items, int Total)> GetPagedAsync(GovernmentEntityListQuery query, bool deletedOnly, CancellationToken ct);
    Task<EntidadGubernamental?> GetByIdAsync(Guid id, bool includeDeleted, CancellationToken ct);
    Task<bool> ActiveNameExistsAsync(string normalizedName, Guid? excludingId, CancellationToken ct);
    Task<HashSet<string>> GetAllNormalizedNamesAsync(CancellationToken ct);
    void Add(EntidadGubernamental entity); void SetOriginalRowVersion(EntidadGubernamental entity, byte[] version);
}
