namespace Portfolio.Domain.Models.Common;

public sealed record PaginationResult<TModel> : ICollectionResponse<TModel>
{
    public List<TModel> Items { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }

    private int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;


    public static PaginationResult<TModel> Create(List<TModel> items, int page, int pageSize, int totalCount)
    {

        return new PaginationResult<TModel>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
