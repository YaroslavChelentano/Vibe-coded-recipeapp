using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RecipeWorld.Tests;

public class RecipeWorldApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", "Data Source=integration_test.db");
        builder.UseSetting("Jwt:Key", "TestSecretKeyAtLeast32CharactersLong!!");
        builder.UseSetting("Jwt:Issuer", "RecipeWorld.Tests");
        builder.UseSetting("Jwt:Audience", "RecipeWorld.App");
        builder.UseSetting("Jwt:ExpirationMinutes", "60");
    }
}
