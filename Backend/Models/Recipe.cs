namespace RecipeWorld.Api.Models;

public class Recipe
{
    public int Id { get; set; }
    public string? ExternalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Area { get; set; }
    public string? Category { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Instructions { get; set; }
    public string Source { get; set; } = "external"; // "external" or "custom"
    public string? IngredientsJson { get; set; }

    public ICollection<UserRecipeFavorite> SavedByUsers { get; set; } = new List<UserRecipeFavorite>();
}
