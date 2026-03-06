using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeWorld.Api.Models;
using RecipeWorld.Api.Services;

namespace RecipeWorld.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuggestionsController : ControllerBase
{
    private readonly ISuggestionService _suggestions;

    public SuggestionsController(ISuggestionService suggestions)
    {
        _suggestions = suggestions;
    }

    [HttpPost("from-ingredients")]
    public async Task<ActionResult<IReadOnlyList<SuggestionResultDto>>> FromIngredients([FromBody] FromIngredientsRequest request, CancellationToken ct)
    {
        var ingredients = request?.Ingredients ?? new List<string>();
        var maxResults = request?.MaxResults ?? 15;
        var list = await _suggestions.GetSuggestionsFromIngredientsAsync(ingredients, maxResults, ct);
        return Ok(list);
    }
}

public class FromIngredientsRequest
{
    public List<string>? Ingredients { get; set; }
    public int MaxResults { get; set; } = 15;
}
