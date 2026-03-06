namespace RecipeWorld.Api.Models;

public class SuggestionResultDto
{
    public RecipeDto Recipe { get; set; } = null!;
    public int Score { get; set; }
    public List<string> MatchingIngredients { get; set; } = new();
    public List<string> MissingIngredients { get; set; } = new();
}
