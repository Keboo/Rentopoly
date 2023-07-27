using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Rentopoly.Data;

namespace Rentopoly.Rentals;

public partial class RentalItemViewModel
{
    private int RentalId { get; }
    public string LoanedTo { get; }
    public DateTime LoanedOn { get; }
    public DateTime? ReturnedOn { get; }
    private BoardGameContext BoardGameContext { get; }
    private IMessenger Messenger { get; }

    public RentalItemViewModel(Rental rental, BoardGameContext boardGameContext, IMessenger messenger)
    {
        RentalId = rental.Id;
        LoanedTo = rental.LoanedTo;
        LoanedOn = rental.LoanedOn;
        ReturnedOn = rental.ReturnedOn;
        BoardGameContext = boardGameContext;
        Messenger = messenger;
    }

    [RelayCommand]
    public async Task OnReturned()
    {
        if (await BoardGameContext.Rentals.FindAsync(RentalId) is { } foundRental)
        {
            foundRental.ReturnedOn = DateTime.Now;
            await BoardGameContext.SaveChangesAsync();
            Messenger.Send(new RentalUpdates(foundRental));
        }
    }
}

public record class RentalUpdates(Rental Rental);
