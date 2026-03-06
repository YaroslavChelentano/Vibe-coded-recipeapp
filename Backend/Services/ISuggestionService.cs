using RecipeWorld.Api.Models;

namespace RecipeWorld.Api.Services;

public interface ISuggestionService
{
    Task<IReadOnlyList<SuggestionResultDto>> GetSuggestionsFromIngredientsAsync(IEnumerable<string> ingredients, int maxResults = 15, CancellationToken ct = default);
}
