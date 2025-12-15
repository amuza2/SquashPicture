using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using SquashPicture.Services;
using SquashPicture.Services.Interfaces;
using SquashPicture.ViewModels;
using SquashPicture.Views;

namespace SquashPicture;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            
            var services = new ServiceCollection();
            ConfigureServices(services, mainWindow);
            var serviceProvider = services.BuildServiceProvider();
            
            var viewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
            mainWindow.DataContext = viewModel;
            
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services, MainWindow mainWindow)
    {
        services.AddSingleton<IFileDialogService>(new FileDialogService(mainWindow));
        services.AddSingleton<MainWindowViewModel>();
    }
}