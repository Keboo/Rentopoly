namespace Rentopoly.Rentals;

/// <summary>
/// Interaction logic for ViewRentalsView.xaml
/// </summary>
public partial class ViewRentalsView
{
    public ViewRentalsView()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is ViewRentalsViewModel viewModel)
        {
            await viewModel.RefreshCommand.ExecuteAsync(null);
        }
    }
}
