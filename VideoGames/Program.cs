using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using VideoGames.Data;
using VideoGames.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add SQLite DbContext
builder.Services.AddDbContext<VideoGamesContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Game Endpoints
app.MapGet("/games", async (VideoGamesContext db) =>
    await db.Games.Include(g => g.Genre).ToListAsync());

app.MapGet("/games/{id}", async (VideoGamesContext db, int id) =>
{
    var game = await db.Games.Include(g => g.Genre).FirstOrDefaultAsync(g => g.Id == id);
    return game == null ? Results.NotFound("Game not found") : Results.Ok(game);
});

app.MapPost("/games", async (VideoGamesContext db, Game game) =>
{
    await db.Games.AddAsync(game);
    await db.SaveChangesAsync();
});

//GameGenre Endpoints 
app.MapGet("gamegenres", async (VideoGamesContext db) =>
    await db.GameGenres.ToListAsync());

app.MapGet("/gamegenres/{id}", async (VideoGamesContext db, int id) =>
{
    var gameGenre = await db.GameGenres.FirstOrDefaultAsync(g => g.Id == id);
    return gameGenre == null ? Results.NotFound("Game Genre not found") : Results.Ok(gameGenre);
});

app.Run();

public partial class Program
{
}