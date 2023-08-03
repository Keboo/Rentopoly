using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MaterialDesignThemes.Wpf;

using Rentopoly.Data;
using Rentopoly.Games;

namespace Rentopoly.Rentals;

public partial class AddRentalViewModel : ObservableObject
{
    private BoardGameContext DbContext { get; }
    public ISnackbarMessageQueue MessageQueue { get; }
    private Dispatcher Dispatcher { get; }
    public ObservableCollection<BoardGame> BoardGames { get; } = new();

    public ObservableCollection<BoardGame> SelectedGames { get; } = new();

    [ObservableProperty]
    private BoardGame? _selectedBoardGame;

    partial void OnSelectedBoardGameChanged(BoardGame? value)
    {
        if (value is not null)
        {
            SelectedGames.Add(value);
        }
        Dispatcher.BeginInvoke(() => SelectedBoardGame = null);
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string? _loanedTo;

    public AddRentalViewModel(BoardGameContext dbContext, ISnackbarMessageQueue messageQueue, Dispatcher dispatcher)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        MessageQueue = messageQueue ?? throw new ArgumentNullException(nameof(messageQueue));
        Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        BindingOperations.EnableCollectionSynchronization(BoardGames, new());
        BindingOperations.EnableCollectionSynchronization(SelectedGames, new());
    }

    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private async Task OnSubmit()
    {
        if (string.IsNullOrWhiteSpace(LoanedTo))
        {
            return;
        }
        Rental newRental = new()
        {
            LoanedOn = DateTime.Today,
            LoanedTo = LoanedTo
        };
        DbContext.Rentals.Add(newRental);
        await DbContext.SaveChangesAsync();
        LoanedTo = string.Empty;
        MessageQueue.Enqueue("Saved!");
    }

    private bool CanSubmit() => !string.IsNullOrWhiteSpace(LoanedTo);

    public async Task LoadGamesAsync()
    {
        BoardGames.Clear();
        await foreach (BoardGame game in DbContext.BoardGames)
        {
            BoardGames.Add(game);
        }
    }
}
