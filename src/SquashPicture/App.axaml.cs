using System;
using Avalonia;
using Avalonia.Controls;
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
    private MainWindow? _mainWindow;
    private MainWindowViewModel? _viewModel;
    private NativeMenuItem? _toggleMenuItem;
    private ISettingsService? _settingsService;
    private const string AppIcon = "icons8-image-100.png";

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _mainWindow = new MainWindow();
            
            var services = new ServiceCollection();
            ConfigureServices(services, _mainWindow);
            var serviceProvider = services.BuildServiceProvider();
            
            _settingsService = serviceProvider.GetRequiredService<ISettingsService>();
            _settingsService.Load();
            
            _viewModel = serviceProvider.GetRequiredService<MainWindowViewModel>();
            _mainWindow.DataContext = _viewModel;
            
            RestoreWindowState();
            
            desktop.MainWindow = _mainWindow;
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            
            _mainWindow.Closing += OnMainWindowClosing;
            
            CreateTrayIcon();
            UpdateToggleMenuText();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnMainWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        _mainWindow?.Hide();
        UpdateToggleMenuText();
    }

    private void CreateTrayIcon()
    {
        try
        {
            var statusMenuItem = new NativeMenuItem
            {
                Header = "SquashPicture",
                IsEnabled = false
            };

            var addMenuItem = new NativeMenuItem { Header = "Add and compress" };
            addMenuItem.Click += (_, _) => _viewModel?.AddFilesCommand.Execute(null);

            var recompressMenuItem = new NativeMenuItem { Header = "Recompress" };
            recompressMenuItem.Click += (_, _) => _viewModel?.RecompressCommand.Execute(null);

            _toggleMenuItem = new NativeMenuItem { Header = GetToggleMenuText() };
            _toggleMenuItem.Click += (_, _) =>
            {
                ToggleWindowVisibility();
                UpdateToggleMenuText();
            };

            var quitMenuItem = new NativeMenuItem { Header = "Quit" };
            quitMenuItem.Click += (_, _) => QuitApplication();

            var menu = new NativeMenu();
            menu.Add(statusMenuItem);
            menu.Add(new NativeMenuItemSeparator());
            menu.Add(addMenuItem);
            menu.Add(recompressMenuItem);
            menu.Add(new NativeMenuItemSeparator());
            menu.Add(_toggleMenuItem);
            menu.Add(new NativeMenuItemSeparator());
            menu.Add(quitMenuItem);

            var trayIcon = new TrayIcon
            {
                Icon = LoadWindowIcon($"/Assets/{AppIcon}"),
                ToolTipText = "SquashPicture",
                Menu = menu
            };

            trayIcon.Clicked += (_, _) =>
            {
                ToggleWindowVisibility();
                UpdateToggleMenuText();
            };

            var trayIcons = new TrayIcons { trayIcon };
            TrayIcon.SetIcons(this, trayIcons);
        }
        catch
        {
        }
    }

    private string GetToggleMenuText()
    {
        return (_mainWindow?.IsVisible == true) ? "Hide" : "Show";
    }

    private void UpdateToggleMenuText()
    {
        if (_toggleMenuItem != null)
        {
            _toggleMenuItem.Header = GetToggleMenuText();
        }
    }

    private void ToggleWindowVisibility()
    {
        if (_mainWindow is null) return;

        if (_mainWindow.IsVisible)
        {
            _mainWindow.Hide();
        }
        else
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Activate();
        }
    }

    private void QuitApplication()
    {
        SaveWindowState();
        _settingsService?.Save();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
    
    private void RestoreWindowState()
    {
        if (_mainWindow is null || _settingsService is null) return;
        
        var settings = _settingsService.Settings;
        
        // Restore window size
        if (settings.WindowWidth > 0 && settings.WindowHeight > 0)
        {
            _mainWindow.Width = settings.WindowWidth;
            _mainWindow.Height = settings.WindowHeight;
        }
        
        // Restore window position if valid
        if (settings.WindowX >= 0 && settings.WindowY >= 0)
        {
            _mainWindow.Position = new PixelPoint((int)settings.WindowX, (int)settings.WindowY);
        }
    }
    
    private void SaveWindowState()
    {
        if (_mainWindow is null || _settingsService is null) return;
        
        var settings = _settingsService.Settings;
        
        // Only save if window is in normal state (not minimized/maximized)
        if (_mainWindow.WindowState == WindowState.Normal)
        {
            settings.WindowWidth = _mainWindow.Width;
            settings.WindowHeight = _mainWindow.Height;
            settings.WindowX = _mainWindow.Position.X;
            settings.WindowY = _mainWindow.Position.Y;
        }
    }

    private WindowIcon? LoadWindowIcon(string path)
    {
        try
        {
            var uri = new Uri($"avares://SquashPicture{path}");
            using var stream = Avalonia.Platform.AssetLoader.Open(uri);
            return new WindowIcon(stream);
        }
        catch
        {
            return null;
        }
    }

    private static void ConfigureServices(IServiceCollection services, MainWindow mainWindow)
    {
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IFileDialogService>(sp => 
            new FileDialogService(mainWindow, sp.GetRequiredService<ISettingsService>()));
        services.AddSingleton<ICompressionService, CompressionService>();
        services.AddSingleton<MainWindowViewModel>();
    }
}