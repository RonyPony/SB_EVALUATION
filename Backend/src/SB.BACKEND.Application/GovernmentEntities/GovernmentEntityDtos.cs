using System.ComponentModel.DataAnnotations;
namespace SB.BACKEND.Application.GovernmentEntities;
public static class GovernmentEntityLengths { public const int Name = 300, Category = 150, StatePower = 100, Sector = 150; }
public class CreateGovernmentEntityRequest
{
    [Required, StringLength(GovernmentEntityLengths.Name)] public string Nombre { get; init; } = "";
    [Required, StringLength(GovernmentEntityLengths.Category)] public string Categoria { get; init; } = "";
    [Required, StringLength(GovernmentEntityLengths.StatePower)] public string PoderDelEstado { get; init; } = "";
    [Required, StringLength(GovernmentEntityLengths.Sector)] public string Sector { get; init; } = "";
}
public sealed class UpdateGovernmentEntityRequest : CreateGovernmentEntityRequest { [Required] public string RowVersion { get; init; } = ""; }
public sealed class ChangeGovernmentEntityStatusRequest { public bool Activo { get; init; } [Required] public string RowVersion { get; init; } = ""; }
public sealed class GovernmentEntityListQuery
{
    public string? Search { get; init; } public string? Categoria { get; init; } public string? PoderDelEstado { get; init; }
    public string? Sector { get; init; } public bool? Activo { get; init; }
    [Range(1, int.MaxValue)] public int PageNumber { get; init; } = 1;
    [Range(1, 100)] public int PageSize { get; init; } = 20;
    public bool Descending { get; init; }
}
public sealed record GovernmentEntityResponse(Guid Id, string Nombre, string Categoria, string PoderDelEstado, string Sector,
    bool Activo, bool IsDeleted, DateTimeOffset CreatedAt, DateTimeOffset? UpdatedAt, DateTimeOffset? DeletedAt,
    Guid? CreatedBy, Guid? UpdatedBy, Guid? DeletedBy, string RowVersion);
