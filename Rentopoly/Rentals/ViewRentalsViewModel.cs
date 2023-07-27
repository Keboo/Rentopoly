using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.EntityFrameworkCore;

using Rentopoly.Data;

namespace Rentopoly.Rentals;

public partial class ViewRentalsViewModel : ObservableObject, IRecipient<RentalUpdates>
{
    private BoardGameContext Context { get; }
    private Func<Rental, RentalItemViewModel> RentalViewModelFactory { get; }
    public ObservableCollection<RentalItemViewModel> Rentals { get; } = new();

    public ViewRentalsViewModel(BoardGameContext context, IMessenger messenger, 
        Func<Rental, RentalItemViewModel> rentalViewModelFactory)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        RentalViewModelFactory = rentalViewModelFactory;
        messenger.RegisterAll(this);
        BindingOperations.EnableCollectionSynchronization(Rentals, new());
    }

    [RelayCommand]
    public async Task OnRefresh()
    {
        Rentals.Clear();
        //TODO: This just here to simulate a delay
        await Task.Delay(2_000);
        await foreach (var rental in Context.Rentals.Where(x => x.ReturnedOn == null).AsAsyncEnumerable())
        {
            Rentals.Add(RentalViewModelFactory(rental));
        }
    }

    public async void Receive(RentalUpdates message)
    {
        await OnRefresh();
    }
}
