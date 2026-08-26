namespace SB.BACKEND.Application.Common;
public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int PageNumber, int PageSize, int TotalRecords)
{ public int TotalPages => TotalRecords == 0 ? 0 : (int)Math.Ceiling(TotalRecords / (double)PageSize); }
