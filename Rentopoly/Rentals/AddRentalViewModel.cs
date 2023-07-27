using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Rentopoly.Data;

namespace Rentopoly.Rentals;

public partial class AddRentalViewModel : ObservableObject
{
    private BoardGameContext DbContext { get; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string? _loanedTo;

    public AddRentalViewModel(BoardGameContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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
    }

    private bool CanSubmit() => !string.IsNullOrWhiteSpace(LoanedTo);
}
