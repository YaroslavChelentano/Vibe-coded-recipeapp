using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace RecipeWorld.Tests;

public class IntegrationTests : IClassFixture<RecipeWorldApplicationFactory>
{
    private readonly HttpClient _client;

    public IntegrationTests(RecipeWorldApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ReturnsTokenAndUser()
    {
        var email = $"test{Guid.NewGuid():N}@test.com";
        var res = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            email,
            password = "Test123!",
            confirmPassword = "Test123!",
            displayName = "Test User"
        });
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body?.Token);
        Assert.Equal(email, body.Email);
        Assert.Equal("Test User", body.DisplayName);
    }

    [Fact]
    public async Task Login_WithSeededUser_ReturnsToken()
    {
        var res = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "demo@recipes.com",
            password = "Demo123!"
        });
        res.EnsureSuccessStatusCode();
        var body = await res.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body?.Token);
        Assert.Equal("demo@recipes.com", body.Email);
    }

    [Fact]
    public async Task Login_InvalidCredentials_Returns401()
    {
        var res = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "demo@recipes.com",
            password = "WrongPassword"
        });
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task GetRecipesSearch_ReturnsResults()
    {
        var res = await _client.GetAsync("/api/recipes/search?query=chicken");
        res.EnsureSuccessStatusCode();
        var list = await res.Content.ReadFromJsonAsync<RecipeDto[]>();
        Assert.NotNull(list);
    }

    [Fact]
    public async Task GetCuisines_ReturnsList()
    {
        var res = await _client.GetAsync("/api/recipes/cuisines");
        res.EnsureSuccessStatusCode();
        var list = await res.Content.ReadFromJsonAsync<string[]>();
        Assert.NotNull(list);
        Assert.Contains("Italian", list);
    }

    [Fact]
    public async Task GetMyRecipes_WithoutAuth_Returns401()
    {
        var res = await _client.GetAsync("/api/user/recipes");
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Login_SaveRecipe_GetMyRecipes_ReturnsSaved()
    {
        var loginRes = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "demo@recipes.com",
            password = "Demo123!"
        });
        loginRes.EnsureSuccessStatusCode();
        var auth = await loginRes.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth?.Token);

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.Token);
        var saveRes = await _client.PostAsJsonAsync("/api/user/recipes/save", new
        {
            externalId = "52770",
            title = "Spaghetti Bolognese",
            area = "Italian",
            thumbnailUrl = "https://example.com/img.jpg"
        });
        saveRes.EnsureSuccessStatusCode();

        var myRes = await _client.GetAsync("/api/user/recipes");
        myRes.EnsureSuccessStatusCode();
        var myRecipes = await myRes.Content.ReadFromJsonAsync<RecipeDto[]>();
        Assert.NotNull(myRecipes);
        Assert.True(myRecipes.Length >= 1);
    }

    private class AuthResponse
    {
        public string Token { get; set; } = "";
        public string Email { get; set; } = "";
        public string DisplayName { get; set; } = "";
    }

    private class RecipeDto
    {
        public int Id { get; set; }
        public string? ExternalId { get; set; }
        public string Title { get; set; } = "";
    }
}
