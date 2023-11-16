using System.Collections.ObjectModel;
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

    private CancellationTokenSource? _refreshCancellation;

    public ViewRentalsViewModel(BoardGameContext context, IMessenger messenger, 
        Func<Rental, RentalItemViewModel> rentalViewModelFactory)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        RentalViewModelFactory = rentalViewModelFactory;
        messenger.RegisterAll(this);
        BindingOperations.EnableCollectionSynchronization(Rentals, new());
    }

    [RelayCommand]
    private async Task OnRefresh()
    {
        CancellationTokenSource cts = new();
        Interlocked.Exchange(ref _refreshCancellation, cts)?.Cancel();
        try
        {
            Rentals.Clear();
            //TODO: This just here to simulate a delay
            await Task.Delay(2_000, cts.Token);
            await foreach (var rental in Context.Rentals.Where(x => x.ReturnedOn == null).AsAsyncEnumerable().WithCancellation(cts.Token))
            {
                Rentals.Add(RentalViewModelFactory(rental));
            }
        }
        catch(OperationCanceledException)
        { }
    }

    public async void Receive(RentalUpdates message)
    {
        await RefreshCommand.ExecuteAsync(null);
    }
}
