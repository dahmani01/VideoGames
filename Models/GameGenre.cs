using System.ComponentModel.DataAnnotations;

namespace VideoGames.Models;

public class GameGenre
{
    public int Id { get; set; }
    
    [MaxLength(150)]  
    [Required]
    public string Name { get; set; } = string.Empty ;

    public List<Game> Games { get; set; } = []; 
}