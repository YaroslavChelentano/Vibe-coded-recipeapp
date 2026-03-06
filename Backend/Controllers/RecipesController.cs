using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeWorld.Api.Data;
using RecipeWorld.Api.Models;
using RecipeWorld.Api.Services;

namespace RecipeWorld.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly IExternalRecipeClient _external;
    private readonly AppDbContext _db;

    public RecipesController(IExternalRecipeClient external, AppDbContext db)
    {
        _external = external;
        _db = db;
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<RecipeDto>>> Search([FromQuery] string query, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query)) return Ok(Array.Empty<RecipeDto>());
        var list = await _external.SearchByNameAsync(query.Trim(), ct);
        return Ok(list);
    }

    [HttpGet("cuisines")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetCuisines(CancellationToken ct)
    {
        var list = await _external.GetCuisinesAsync(ct);
        return Ok(list);
    }

    [HttpGet("by-cuisine")]
    public async Task<ActionResult<IReadOnlyList<RecipeDto>>> GetByCuisine([FromQuery] string area, [FromQuery] int count = 20, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(area)) return BadRequest(new { message = "Area is required." });
        var list = await _external.GetByAreaAsync(area.Trim(), count, ct);
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RecipeDto>> GetById(string id, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(id)) return BadRequest();
        if (int.TryParse(id, out var internalId))
        {
            var fromDb = await _db.Recipes.FindAsync(new object[] { internalId }, ct);
            if (fromDb != null)
                return Ok(new RecipeDto
                {
                    Id = fromDb.Id,
                    ExternalId = fromDb.ExternalId,
                    Title = fromDb.Title,
                    Area = fromDb.Area,
                    Category = fromDb.Category,
                    ThumbnailUrl = fromDb.ThumbnailUrl,
                    Instructions = fromDb.Instructions,
                    Source = fromDb.Source,
                    IngredientsJson = fromDb.IngredientsJson,
                    IsSaved = false
                });
        }
        var recipe = await _external.GetByIdAsync(id, ct);
        if (recipe == null) return NotFound();
        return Ok(recipe);
    }
}
