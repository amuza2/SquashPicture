using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using SquashPicture.ViewModels;

namespace SquashPicture.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainWindowViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void OnGitHubLinkClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}