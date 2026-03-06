using System.Text.Json;
using RecipeWorld.Api.Models;

namespace RecipeWorld.Api.Services;

public class TheMealDbClient : IExternalRecipeClient
{
    private const string BaseUrl = "https://www.themealdb.com/api/json/v1/1";
    private readonly HttpClient _http;

    public TheMealDbClient(HttpClient http)
    {
        _http = http;
        _http.Timeout = TimeSpan.FromSeconds(15);
    }

    public async Task<IReadOnlyList<RecipeDto>> SearchByNameAsync(string query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query)) return Array.Empty<RecipeDto>();
        var url = $"{BaseUrl}/search.php?s={Uri.EscapeDataString(query.Trim())}";
        var list = await GetMealsListAsync<MealDbMealDetail>(url, ct);
        return list.Select(ToRecipeDto).ToList();
    }

    public async Task<IReadOnlyList<string>> GetCuisinesAsync(CancellationToken ct = default)
    {
        var url = $"{BaseUrl}/list.php?a=list";
        var list = await GetMealsListAsync<MealDbAreaItem>(url, ct);
        return list.Select(m => m.StrArea ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList();
    }

    public async Task<IReadOnlyList<RecipeDto>> GetByAreaAsync(string area, int count = 20, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(area)) return Array.Empty<RecipeDto>();
        var url = $"{BaseUrl}/filter.php?a={Uri.EscapeDataString(area.Trim())}";
        var list = await GetMealsListAsync<MealDbMealListItem>(url, ct);
        var ids = list.Take(count).Select(m => m.IdMeal).Where(id => !string.IsNullOrEmpty(id)).ToList();
        var results = new List<RecipeDto>();
        foreach (var id in ids)
        {
            var detail = await GetByIdAsync(id!, ct);
            if (detail != null) results.Add(detail);
        }
        return results;
    }

    public async Task<RecipeDto?> GetByIdAsync(string externalId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(externalId)) return null;
        var url = $"{BaseUrl}/lookup.php?i={Uri.EscapeDataString(externalId.Trim())}";
        var list = await GetMealsListAsync<MealDbMealDetail>(url, ct);
        var meal = list.FirstOrDefault();
        return meal == null ? null : ToRecipeDto(meal);
    }

    public async Task<IReadOnlyList<RecipeDto>> SearchByIngredientsAsync(IEnumerable<string> ingredients, int maxResults = 20, CancellationToken ct = default)
    {
        var normalized = ingredients.Select(i => i.Trim()).Where(s => !string.IsNullOrEmpty(s)).Take(3).ToList();
        if (normalized.Count == 0) return Array.Empty<RecipeDto>();

        var seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var results = new List<RecipeDto>();
        foreach (var ing in normalized)
        {
            var url = $"{BaseUrl}/filter.php?i={Uri.EscapeDataString(ing)}";
            var list = await GetMealsListAsync<MealDbMealListItem>(url, ct);
            foreach (var m in list)
            {
                if (string.IsNullOrEmpty(m.IdMeal) || seenIds.Contains(m.IdMeal)) continue;
                seenIds.Add(m.IdMeal);
                var detail = await GetByIdAsync(m.IdMeal, ct);
                if (detail != null)
                {
                    results.Add(detail);
                    if (results.Count >= maxResults) return results;
                }
            }
        }
        return results;
    }

    private async Task<List<T>> GetMealsListAsync<T>(string url, CancellationToken ct)
    {
        var response = await _http.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(ct);
        var root = JsonSerializer.Deserialize<MealDbListResponse<T>>(json);
        return root?.Meals ?? new List<T>();
    }

    private static RecipeDto ToRecipeDto(MealDbMealDetail m)
    {
        var ingredients = GetIngredientsList(m);
        return new RecipeDto
        {
            ExternalId = m.IdMeal,
            Title = m.StrMeal ?? "",
            Area = m.StrArea,
            Category = m.StrCategory,
            ThumbnailUrl = m.StrMealThumb,
            Instructions = m.StrInstructions,
            Source = "external",
            IngredientsJson = ingredients.Count > 0 ? JsonSerializer.Serialize(ingredients) : null,
            IsSaved = false
        };
    }

    private static List<string> GetIngredientsList(MealDbMealDetail m)
    {
        var list = new List<string>();
        var ext = m.ExtensionData;
        if (ext == null) return list;

        for (var i = 1; i <= 20; i++)
        {
            var keyIng = $"strIngredient{i}";
            if (!ext.TryGetValue(keyIng, out var ingEl)) continue;
            var ing = ingEl.GetString()?.Trim();
            if (string.IsNullOrEmpty(ing)) continue;

            var keyMeasure = $"strMeasure{i}";
            var measure = ext.TryGetValue(keyMeasure, out var measureEl) ? measureEl.GetString()?.Trim() : null;
            list.Add(string.IsNullOrEmpty(measure) ? ing : $"{ing} - {measure}");
        }
        return list;
    }
}

