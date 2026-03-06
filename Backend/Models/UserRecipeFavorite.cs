namespace RecipeWorld.Api.Models;

public class UserRecipeFavorite
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}
