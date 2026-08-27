using System.ComponentModel.DataAnnotations;

namespace SB.BACKEND.Application.GovernmentEntities;

public static class GovernmentEntityLengths
{
    public const int NAME = 300,
        CATEGORY = 150,
        STATE_POWER = 100,
        SECTOR = 150;
}

public class CreateGovernmentEntityRequest
{
    [Required, StringLength(GovernmentEntityLengths.NAME)]
    public string Nombre { get; init; } = "";

    [Required, StringLength(GovernmentEntityLengths.CATEGORY)]
    public string Categoria { get; init; } = "";

    [Required, StringLength(GovernmentEntityLengths.STATE_POWER)]
    public string PoderDelEstado { get; init; } = "";

    [Required, StringLength(GovernmentEntityLengths.SECTOR)]
    public string Sector { get; init; } = "";
}

public sealed class UpdateGovernmentEntityRequest : CreateGovernmentEntityRequest
{
    [Required]
    public string RowVersion { get; init; } = "";
}

public sealed class ChangeGovernmentEntityStatusRequest
{
    public bool Activo { get; init; }

    [Required]
    public string RowVersion { get; init; } = "";
}

public sealed class GovernmentEntityListQuery
{
    public string? Search { get; init; }
    public string? Categoria { get; init; }
    public string? PoderDelEstado { get; init; }
    public string? Sector { get; init; }
    public bool? Activo { get; init; }

    [Range(1, int.MaxValue)]
    public int PageNumber { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;
    public bool Descending { get; init; }
}

public sealed record GovernmentEntityResponse(
    Guid Id,
    string Nombre,
    string Categoria,
    string PoderDelEstado,
    string Sector,
    bool Activo,
    bool IsDeleted,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    DateTimeOffset? DeletedAt,
    Guid? CreatedBy,
    Guid? UpdatedBy,
    Guid? DeletedBy,
    string RowVersion
);
