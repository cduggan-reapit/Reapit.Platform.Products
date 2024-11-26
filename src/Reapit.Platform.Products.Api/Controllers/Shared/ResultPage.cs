using System.Text.Json.Serialization;

namespace Reapit.Platform.Products.Api.Controllers.Shared;

/// <summary>Container for a paged collection result.</summary>
/// <param name="Data">Collection of models returned.</param>
/// <param name="Count">The number of models returned for this page.</param>
/// <param name="Cursor">The cursor for the next page of results.</param>
/// <typeparam name="TModel">The type of model.</typeparam>
public record ResultPage<TModel>(
    [property: JsonPropertyName("data")] IEnumerable<TModel> Data,
    [property: JsonPropertyName("item_count")] int Count,
    [property: JsonPropertyName("next_cursor")] long Cursor) 
    where TModel : class;