using System.Windows;

namespace Rentopoly.Rentals;

/// <summary>
/// Interaction logic for AddRentalView.xaml
/// </summary>
public partial class AddRentalView
{
    public AddRentalView()
    {
        Loaded += AddRentalView_Loaded;
        InitializeComponent();
        GameDropDown.AddHandler(MouseDownEvent, new RoutedEventHandler(OnMouseDown), true);
    }

    private async void AddRentalView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is AddRentalViewModel viewModel)
        {
            await viewModel.LoadGamesAsync();
        }
    }

    private void OnMouseDown(object sender, RoutedEventArgs e)
    {
        if (e.Handled == true && e.OriginalSource != null)
        {

        }
    }
}
