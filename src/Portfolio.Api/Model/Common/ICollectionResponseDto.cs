namespace Portfolio.Api.Model.Common;

public interface ICollectionResponseDto<TModel>
{
    List<TModel> Items { get; init; }
}
