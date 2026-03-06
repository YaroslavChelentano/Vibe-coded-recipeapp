using RecipeWorld.Api.Models;

namespace RecipeWorld.Api.Services;

public interface IExternalRecipeClient
{
    Task<IReadOnlyList<RecipeDto>> SearchByNameAsync(string query, CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetCuisinesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<RecipeDto>> GetByAreaAsync(string area, int count = 20, CancellationToken ct = default);
    Task<RecipeDto?> GetByIdAsync(string externalId, CancellationToken ct = default);
    Task<IReadOnlyList<RecipeDto>> SearchByIngredientsAsync(IEnumerable<string> ingredients, int maxResults = 20, CancellationToken ct = default);
}
