using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.EntityFrameworkCore;

using Rentopoly.Data;

namespace Rentopoly.Rentals;

public partial class ViewRentalsViewModel : ObservableObject
{
    private BoardGameContext Context { get; }

    public ObservableCollection<RentalItemViewModel> Rentals { get; } = new();

    public ViewRentalsViewModel(BoardGameContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        BindingOperations.EnableCollectionSynchronization(Rentals, new());
    }

    [RelayCommand]
    public async Task OnRefresh()
    {
        Rentals.Clear();
        //TODO: This just here to simulate a delay
        await Task.Delay(1_000);
        await foreach (var rental in Context.Rentals.Where(x => x.ReturnedOn == null).AsAsyncEnumerable())
        {
            Rentals.Add(new RentalItemViewModel(rental));
        }
    }
}

public class RentalItemViewModel
{
    public string LoanedTo { get; }
    public DateTime LoanedOn { get; }
    public DateTime? ReturnedOn { get; }

    public RentalItemViewModel(Rental rental)
    {
        LoanedTo = rental.LoanedTo;
        LoanedOn = rental.LoanedOn;
        ReturnedOn = rental.ReturnedOn;
    }
}
