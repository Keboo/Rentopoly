using Rentopoly.Data;

namespace Rentopoly.Games;

public class GameViewModel
{
    public string Name { get; }
    public string Description { get; }
    public int MinPlayers { get; }
    public int MaxPlayers { get; }

    public GameViewModel(BoardGame game)
    {
        ArgumentNullException.ThrowIfNull(game);
        Name = game.Name;
        Description = game.Description;
        MinPlayers = game.MinPlayers;
        MaxPlayers = game.MaxPlayers;
    }
}
