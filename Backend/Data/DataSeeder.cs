using Microsoft.EntityFrameworkCore;
using RecipeWorld.Api.Models;

namespace RecipeWorld.Api.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync()) return;

        var demoHash = BCrypt.Net.BCrypt.HashPassword("Demo123!");
        var chefHash = BCrypt.Net.BCrypt.HashPassword("Chef123!");

        var demo = new User
        {
            Email = "demo@recipes.com",
            PasswordHash = demoHash,
            DisplayName = "Demo User",
            CreatedAt = DateTime.UtcNow
        };
        var chef = new User
        {
            Email = "chef@worldcuisine.com",
            PasswordHash = chefHash,
            DisplayName = "World Chef",
            CreatedAt = DateTime.UtcNow
        };
        db.Users.AddRange(demo, chef);
        await db.SaveChangesAsync();

        var recipes = new[]
        {
            new Recipe { ExternalId = "52982", Title = "Spaghetti alla Carbonara", Area = "Italian", ThumbnailUrl = "https://www.themealdb.com/images/media/meals/llcbn01574260722.jpg", Source = "external" },
            new Recipe { ExternalId = "52839", Title = "Chilli prawn linguine", Area = "Italian", ThumbnailUrl = "https://www.themealdb.com/images/media/meals/usywpp1511189717.jpg", Source = "external" },
            new Recipe { ExternalId = "52814", Title = "Thai Green Curry", Area = "Thai", ThumbnailUrl = "https://www.themealdb.com/images/media/meals/sstssx1487349585.jpg", Source = "external" },
            new Recipe { ExternalId = "53064", Title = "Fettuccine Alfredo", Area = "Italian", ThumbnailUrl = "https://www.themealdb.com/images/media/meals/0jv5gx1661040802.jpg", Source = "external" }
        };
        db.Recipes.AddRange(recipes);
        await db.SaveChangesAsync();

        var demoUser = await db.Users.FirstAsync(u => u.Email == "demo@recipes.com");
        var chefUser = await db.Users.FirstAsync(u => u.Email == "chef@worldcuisine.com");
        var r1 = await db.Recipes.FirstAsync(r => r.ExternalId == "52982");
        var r2 = await db.Recipes.FirstAsync(r => r.ExternalId == "52839");
        var r3 = await db.Recipes.FirstAsync(r => r.ExternalId == "52814");
        var r4 = await db.Recipes.FirstAsync(r => r.ExternalId == "53064");

        db.UserRecipeFavorites.AddRange(
            new UserRecipeFavorite { UserId = demoUser.Id, RecipeId = r1.Id },
            new UserRecipeFavorite { UserId = demoUser.Id, RecipeId = r2.Id },
            new UserRecipeFavorite { UserId = chefUser.Id, RecipeId = r3.Id },
            new UserRecipeFavorite { UserId = chefUser.Id, RecipeId = r4.Id }
        );
        await db.SaveChangesAsync();
    }
}
