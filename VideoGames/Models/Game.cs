using System.ComponentModel.DataAnnotations;

namespace VideoGames.Models;

public class Game
{
    public int Id { get; set; }
    
    [MaxLength(150)]
    [Required]
    public string Name { get; set; }
   
    public string? Description { get; set; }
    
    public int GenreId { get; set; }
    public GameGenre Genre { get; set; }
}