using CommunityToolkit.Mvvm.Messaging;

using Rentopoly.Data;

namespace Rentopoly.Games;

/// <summary>
/// Interaction logic for CatalogView.xaml
/// </summary>
public partial class CatalogView
{
    public CatalogView()
    {
        InitializeComponent();
    }


    private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is CatalogViewModel viewModel)
        {
            await viewModel.LoadGamesAsync();
        }
    }
}
