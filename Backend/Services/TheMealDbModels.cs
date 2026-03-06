using System.Text.Json.Serialization;

namespace RecipeWorld.Api.Services;

public class MealDbListResponse<T>
{
    [JsonPropertyName("meals")]
    public List<T>? Meals { get; set; }
}

public class MealDbMealListItem
{
    [JsonPropertyName("idMeal")]
    public string? IdMeal { get; set; }
    [JsonPropertyName("strMeal")]
    public string? StrMeal { get; set; }
    [JsonPropertyName("strMealThumb")]
    public string? StrMealThumb { get; set; }
    [JsonPropertyName("strArea")]
    public string? StrArea { get; set; }
    [JsonPropertyName("strCategory")]
    public string? StrCategory { get; set; }
}

/// <summary>
/// TheMealDB API returns each meal with fixed fields strIngredient1..20 and strMeasure1..20.
/// We only map the core fields here; ingredients/measures are read from extension data.
/// </summary>
public class MealDbMealDetail
{
    [JsonPropertyName("idMeal")]
    public string? IdMeal { get; set; }
    [JsonPropertyName("strMeal")]
    public string? StrMeal { get; set; }
    [JsonPropertyName("strArea")]
    public string? StrArea { get; set; }
    [JsonPropertyName("strCategory")]
    public string? StrCategory { get; set; }
    [JsonPropertyName("strMealThumb")]
    public string? StrMealThumb { get; set; }
    [JsonPropertyName("strInstructions")]
    public string? StrInstructions { get; set; }

    [JsonExtensionData]
    public Dictionary<string, System.Text.Json.JsonElement>? ExtensionData { get; set; }
}

public class MealDbAreaItem
{
    [JsonPropertyName("strArea")]
    public string? StrArea { get; set; }
}
