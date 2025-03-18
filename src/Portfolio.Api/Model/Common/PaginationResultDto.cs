namespace Portfolio.Api.Model.Common;

public sealed record PaginationResultDto<TModel> : ICollectionResponseDto<TModel>, ILinksResponseDto
{
    public List<TModel> Items { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public List<LinkDto> Links { get; set; }

    private int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;


    public static PaginationResultDto<TModel> Create(List<TModel> items, int page, int pageSize, int totalCount)
    {

        return new PaginationResultDto<TModel>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
