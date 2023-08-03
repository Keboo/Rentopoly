using System.Windows.Input;

using Rentopoly.Games;
using Rentopoly.Rentals;

namespace Rentopoly;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow(
        ViewRentalsViewModel viewRentalsViewModel,
        AddRentalViewModel addRentalViewModel,
        CatalogViewModel catalogViewModel)
    {
        InitializeComponent();

        AddRentalView.DataContext = addRentalViewModel;
        ViewRentalsView.DataContext = viewRentalsViewModel;
        GameCatalogView.DataContext = catalogViewModel;

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, OnClose));
    }

    private void OnClose(object sender, ExecutedRoutedEventArgs e)
    {
        Close();
    }
}
