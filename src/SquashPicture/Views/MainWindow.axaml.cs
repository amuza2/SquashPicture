using System.Diagnostics;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using SquashPicture.ViewModels;

namespace SquashPicture.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SetupQuitShortcut();
    }

    public MainWindow(MainWindowViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    private void SetupQuitShortcut()
    {
        foreach (var binding in KeyBindings)
        {
            if (binding.Gesture is KeyGesture gesture && 
                gesture.Key == Key.Q && 
                gesture.KeyModifiers == KeyModifiers.Control)
            {
                binding.Command = new QuitCommand(this);
                break;
            }
        }
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

    private class QuitCommand : ICommand
    {
        private readonly Window _window;

        public QuitCommand(Window window)
        {
            _window = window;
        }

#pragma warning disable CS0067
        public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _window.Close();
    }
}