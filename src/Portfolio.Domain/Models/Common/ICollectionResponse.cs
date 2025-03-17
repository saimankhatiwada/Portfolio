namespace Portfolio.Domain.Models.Common;

public interface ICollectionResponse<TModel>
{
    List<TModel> Items { get; init; }
}
