using Microsoft.EntityFrameworkCore;
using VideoGames.Models;

namespace VideoGames.Data;

public class VideoGamesContext : DbContext
{
    public VideoGamesContext(DbContextOptions<VideoGamesContext> options) : base(options)
    {
    }
    
    public DbSet<Game> Games { get; set; }
    public DbSet<GameGenre> GameGenres { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //seeding Data for test
        modelBuilder.Entity<GameGenre>().HasData(
            new GameGenre { Id = 1, Name = "Action" },
            new GameGenre { Id = 2, Name = "RPG" }
        );
    }
}