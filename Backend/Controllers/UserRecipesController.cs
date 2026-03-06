using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeWorld.Api.Data;
using RecipeWorld.Api.Models;

namespace RecipeWorld.Api.Controllers;

[ApiController]
[Route("api/user/recipes")]
[Authorize]
public class UserRecipesController : ControllerBase
{
    private readonly AppDbContext _db;

    public UserRecipesController(AppDbContext db)
    {
        _db = db;
    }

    private int GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(id ?? "0");
    }

    [HttpGet]
    public async Task<ActionResult<List<RecipeDto>>> GetMyRecipes(CancellationToken ct)
    {
        var userId = GetUserId();
        var recipes = await _db.UserRecipeFavorites
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.SavedAt)
            .Select(f => new RecipeDto
            {
                Id = f.Recipe.Id,
                ExternalId = f.Recipe.ExternalId,
                Title = f.Recipe.Title,
                Area = f.Recipe.Area,
                Category = f.Recipe.Category,
                ThumbnailUrl = f.Recipe.ThumbnailUrl,
                Instructions = f.Recipe.Instructions,
                Source = f.Recipe.Source,
                IngredientsJson = f.Recipe.IngredientsJson,
                IsSaved = true
            })
            .ToListAsync(ct);
        return Ok(recipes);
    }

    [HttpPost("save")]
    public async Task<ActionResult<RecipeDto>> SaveRecipe([FromBody] SaveRecipeRequest request, CancellationToken ct)
    {
        var userId = GetUserId();
        Recipe? recipe;

        if (request.Id.HasValue && request.Id.Value > 0)
        {
            recipe = await _db.Recipes.FindAsync(new object[] { request.Id.Value }, ct);
            if (recipe == null) return NotFound();
        }
        else if (!string.IsNullOrEmpty(request.ExternalId))
        {
            recipe = await _db.Recipes.FirstOrDefaultAsync(r => r.ExternalId == request.ExternalId, ct);
            if (recipe == null)
            {
                recipe = new Recipe
                {
                    ExternalId = request.ExternalId,
                    Title = request.Title,
                    Area = request.Area,
                    Category = request.Category,
                    ThumbnailUrl = request.ThumbnailUrl,
                    Instructions = request.Instructions,
                    IngredientsJson = request.IngredientsJson,
                    Source = "external"
                };
                _db.Recipes.Add(recipe);
                await _db.SaveChangesAsync(ct);
            }
        }
        else
        {
            return BadRequest(new { message = "Provide either Id or ExternalId." });
        }

        var r = recipe!;
        var exists = await _db.UserRecipeFavorites.AnyAsync(
            f => f.UserId == userId && f.RecipeId == r.Id, ct);
        if (exists) return Ok(ToDto(r, true));

        _db.UserRecipeFavorites.Add(new UserRecipeFavorite { UserId = userId, RecipeId = r.Id });
        await _db.SaveChangesAsync(ct);
        return Ok(ToDto(r, true));
    }

    [HttpDelete("save/{recipeId:int}")]
    public async Task<ActionResult> UnsaveRecipe(int recipeId, CancellationToken ct)
    {
        var userId = GetUserId();
        var fav = await _db.UserRecipeFavorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId, ct);
        if (fav == null) return NoContent();
        _db.UserRecipeFavorites.Remove(fav);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }

    private static RecipeDto ToDto(Recipe r, bool isSaved) => new()
    {
        Id = r.Id,
        ExternalId = r.ExternalId,
        Title = r.Title,
        Area = r.Area,
        Category = r.Category,
        ThumbnailUrl = r.ThumbnailUrl,
        Instructions = r.Instructions,
        Source = r.Source,
        IngredientsJson = r.IngredientsJson,
        IsSaved = isSaved
    };
}
