using System.Text.Json;
using RecipeWorld.Api.Models;

namespace RecipeWorld.Api.Services;

public class SuggestionService : ISuggestionService
{
    private readonly IExternalRecipeClient _external;

    public SuggestionService(IExternalRecipeClient external)
    {
        _external = external;
    }

    public async Task<IReadOnlyList<SuggestionResultDto>> GetSuggestionsFromIngredientsAsync(IEnumerable<string> ingredients, int maxResults = 15, CancellationToken ct = default)
    {
        var normalized = ingredients
            .Select(i => i.Trim().ToLowerInvariant())
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .ToList();
        if (normalized.Count == 0) return Array.Empty<SuggestionResultDto>();

        var candidates = await _external.SearchByIngredientsAsync(normalized, maxResults * 2, ct);
        var userSet = new HashSet<string>(normalized, StringComparer.OrdinalIgnoreCase);
        var results = new List<SuggestionResultDto>();

        foreach (var recipe in candidates)
        {
            var recipeIngredients = ParseIngredients(recipe.IngredientsJson);
            var matching = recipeIngredients.Where(ri => userSet.Contains(NormalizeIngredientName(ri))).ToList();
            var missing = recipeIngredients.Where(ri => !userSet.Contains(NormalizeIngredientName(ri))).ToList();
            var score = matching.Count * 2 - Math.Min(missing.Count, 10);
            results.Add(new SuggestionResultDto
            {
                Recipe = recipe,
                Score = score,
                MatchingIngredients = matching,
                MissingIngredients = missing
            });
        }

        return results
            .OrderByDescending(r => r.Score)
            .Take(maxResults)
            .ToList();
    }

    private static List<string> ParseIngredients(string? ingredientsJson)
    {
        if (string.IsNullOrWhiteSpace(ingredientsJson)) return new List<string>();
        try
        {
            var list = JsonSerializer.Deserialize<List<string>>(ingredientsJson);
            return list ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private static string NormalizeIngredientName(string ingredientMeasure)
    {
        var part = ingredientMeasure.Split('-')[0].Trim();
        return part.ToLowerInvariant();
    }
}
