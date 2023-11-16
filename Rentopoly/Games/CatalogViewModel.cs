using System.Collections.ObjectModel;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Rentopoly.Data;

namespace Rentopoly.Games;

public partial class CatalogViewModel : ObservableObject
{
    private BoardGameContext GameContext { get; }
    private IMessenger Messenger { get; }
    public ObservableCollection<GameViewModel> Games { get; } = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveGameCommand))]
    private string? _newGameName;

    [ObservableProperty]
    private bool _drawerOpen;

    public CatalogViewModel(BoardGameContext gameContext, IMessenger messenger)
    {
        GameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
        Messenger = messenger;
        BindingOperations.EnableCollectionSynchronization(Games, new object());
    }

    public async Task LoadGamesAsync()
    {
        Games.Clear();
        await foreach (BoardGame game in GameContext.BoardGames)
        {
            Games.Add(new GameViewModel(game));
        }
    }

    [RelayCommand(CanExecute = nameof(CanSaveGame))]
    private async Task SaveGame()
    {
        BoardGame boardGame = new()
        {
            Name = NewGameName ?? ""
        };

        GameContext.BoardGames.Add(boardGame);
        await GameContext.SaveChangesAsync();

        NewGameName = null;
        DrawerOpen = false;
        await LoadGamesAsync();
    }

    private bool CanSaveGame() => !string.IsNullOrWhiteSpace(NewGameName);
}
