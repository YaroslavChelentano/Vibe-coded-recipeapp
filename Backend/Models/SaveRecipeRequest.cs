namespace RecipeWorld.Api.Models;

public class SaveRecipeRequest
{
    public int? Id { get; set; }
    public string? ExternalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Area { get; set; }
    public string? Category { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Instructions { get; set; }
    public string? IngredientsJson { get; set; }
}
