using Microsoft.EntityFrameworkCore;
using RecipeWorld.Api.Models;

namespace RecipeWorld.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<UserRecipeFavorite> UserRecipeFavorites => Set<UserRecipeFavorite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRecipeFavorite>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RecipeId });
            entity.HasOne(e => e.User).WithMany(u => u.SavedRecipes).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Recipe).WithMany(r => r.SavedByUsers).HasForeignKey(e => e.RecipeId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasIndex(e => e.ExternalId).IsUnique().HasFilter("[ExternalId] IS NOT NULL");
        });
    }
}
