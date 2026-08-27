namespace SB.BACKEND.Application.Common;

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int PageNumber,
    int PageSize,
    int TotalRecords
)
{
    public int TotalPages
    {
        get { return TotalRecords == 0 ? 0 : (int)Math.Ceiling(TotalRecords / (double)PageSize); }
    }
}
