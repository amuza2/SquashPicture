using Avalonia.Controls;
using SquashPicture.Services;
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
}