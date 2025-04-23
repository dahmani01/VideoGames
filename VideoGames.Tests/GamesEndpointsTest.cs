using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VideoGames.Data;
using VideoGames.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace VideoGames.Tests;

public class GamesEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public GamesEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            // Override the DbContext to use an in-memory database for testing
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(IDbContextOptionsConfiguration<VideoGamesContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add an in-memory database
                services.AddDbContext<VideoGamesContext>(options =>
                    options.UseInMemoryDatabase("TestDatabase"));
            });
        });

        _client = _factory.CreateClient();
    }

    private async Task SeedData(VideoGamesContext db)
    {
        // Seed some test data
        var genre = new GameGenre { Id = 1, Name = "Action" };
        var game = new Game { Id = 1, Name = "DOOM", Description = "A fast-paced FPS", GenreId = 1, Genre = genre };

        db.GameGenres.Add(genre);
        db.Games.Add(game);
        await db.SaveChangesAsync();
    }


    [Fact]
    public async Task GetGames_ReturnsListOfGames()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VideoGamesContext>();
        await SeedData(db); // Seed test data

        // Act
        var response = await _client.GetAsync("/games");
        response.EnsureSuccessStatusCode(); // Throws if not 200-299
        var games = await response.Content.ReadFromJsonAsync<List<Game>>();

        // Assert
        Assert.NotNull(games);
        Assert.Single(games); // We seeded 1 game
        Assert.Equal("DOOM", games[0].Name);
        Assert.Equal("Action", games[0].Genre?.Name);
    }

    [Fact]
    public async Task PostGame_CreatesNewGame()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VideoGamesContext>();

        var newGame = new Game
        {
            Id = 2,
            Name = "Hollow Knight",
            Description = "A Metroidvania masterpiece",
            GenreId = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/games", newGame);
        response.EnsureSuccessStatusCode(); // Throws if not 200-299

        // Assert
        var createdGame = await db.Games.FirstOrDefaultAsync(g => g.Name == "Hollow Knight");
        Assert.NotNull(createdGame);
        Assert.Equal("Hollow Knight", createdGame.Name);
        Assert.Equal("A Metroidvania masterpiece", createdGame.Description);
        Assert.Equal(1, createdGame.GenreId);
    }
}