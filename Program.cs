using Microsoft.EntityFrameworkCore;
using VideoGames.Data;
using VideoGames.Models;

var builder = WebApplication.CreateBuilder(args);

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


app.MapGet("/games", async (VideoGamesContext db) =>
    await db.Games.Include(g => g.Genre).ToListAsync());

app.MapGet("/games/{id}", async (VideoGamesContext db, int id) =>
    await db.Games.Include(g => g.Genre).FirstOrDefaultAsync(g => g.Id == id));

app.MapPost("/games", async (VideoGamesContext db, Game game) =>
    await db.Games.AddAsync(game));

app.Run();
