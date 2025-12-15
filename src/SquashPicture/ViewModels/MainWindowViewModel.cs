using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SquashPicture.Models;
using SquashPicture.Services.Interfaces;

namespace SquashPicture.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IFileDialogService _fileDialogService;
    private static readonly string[] SupportedExtensions = [".png", ".jpg", ".jpeg"];

    [ObservableProperty]
    private ObservableCollection<ImageItemViewModel> _images = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RecompressCommand))]
    private bool _hasFiles;

    [ObservableProperty]
    private string _windowTitle = "SquashPicture";

    public MainWindowViewModel(IFileDialogService fileDialogService)
    {
        _fileDialogService = fileDialogService;
        Images.CollectionChanged += (_, _) => UpdateState();
    }

    [RelayCommand]
    private async Task AddFilesAsync()
    {
        var files = await _fileDialogService.OpenFileDialogAsync(
            "Select Images",
            null,
            SupportedExtensions);

        if (files is null)
            return;

        AddFiles(files);
    }

    [RelayCommand(CanExecute = nameof(HasFiles))]
    private void Recompress()
    {
        foreach (var image in Images)
        {
            image.Status = CompressionStatus.Queued;
            image.CompressedSize = 0;
            image.ErrorMessage = null;
            image.IsRecompression = true;
        }
    }

    public void AddFiles(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            if (!IsSupportedFile(path))
                continue;

            if (Images.Any(i => i.FullPath.Equals(path, StringComparison.OrdinalIgnoreCase)))
                continue;

            Images.Add(new ImageItemViewModel(path));
        }
    }

    private void UpdateState()
    {
        HasFiles = Images.Count > 0;
        WindowTitle = HasFiles
            ? $"SquashPicture ({Images.Count} files)"
            : "SquashPicture";
    }

    private static bool IsSupportedFile(string path)
    {
        if (!File.Exists(path))
            return false;

        var ext = Path.GetExtension(path).ToLowerInvariant();
        return SupportedExtensions.Contains(ext);
    }
}
