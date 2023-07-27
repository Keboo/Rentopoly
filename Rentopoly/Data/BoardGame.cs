namespace Rentopoly.Data;

public class BoardGame
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public List<Rental> Rentals { get; } = new();
}