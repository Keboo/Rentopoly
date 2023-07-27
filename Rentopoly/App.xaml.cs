using System.Windows;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.Messaging;

using MaterialDesignThemes.Wpf;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Rentopoly.Data;
using Rentopoly.Rentals;

namespace Rentopoly;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    [STAThread]
    public static void Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();
        host.Start();

        using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        using (BoardGameContext context = scope.ServiceProvider.GetRequiredService<BoardGameContext>())
        {
            context.Database.Migrate();
        }

        App app = new();
        app.InitializeComponent();
        app.MainWindow = host.Services.GetRequiredService<MainWindow>();
        app.MainWindow.Visibility = Visibility.Visible;
        app.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder)
            => configurationBuilder.AddUserSecrets(typeof(App).Assembly))
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<AddRentalViewModel>();
            services.AddSingleton<ViewRentalsViewModel>();

            services.AddSingleton<Func<Rental, RentalItemViewModel>>(serviceProvider =>
            {
                return (Rental rental) => new RentalItemViewModel(rental, 
                        serviceProvider.GetRequiredService<BoardGameContext>(),
                        serviceProvider.GetRequiredService<IMessenger>());
            });

            services.AddSingleton<WeakReferenceMessenger>();
            services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider => provider.GetRequiredService<WeakReferenceMessenger>());

            services.AddSingleton(_ => Current.Dispatcher);

            services.AddDbContext<BoardGameContext>();

            services.AddTransient<ISnackbarMessageQueue>(provider =>
            {
                Dispatcher dispatcher = provider.GetRequiredService<Dispatcher>();
                return new SnackbarMessageQueue(TimeSpan.FromSeconds(2.0), dispatcher);
            });
        });
}
