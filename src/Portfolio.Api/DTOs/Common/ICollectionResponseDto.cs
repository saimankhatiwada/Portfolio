namespace Portfolio.Api.DTOs.Common;

public interface ICollectionResponseDto<TModel>
{
    List<TModel> Items { get; init; }
}
