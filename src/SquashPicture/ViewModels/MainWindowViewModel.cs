using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SquashPicture.Models;
using SquashPicture.Services.Interfaces;

namespace SquashPicture.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IFileDialogService _fileDialogService;
    private readonly ICompressionService _compressionService;
    private CancellationTokenSource? _compressionCts;
    private static readonly string[] SupportedExtensions = [".png", ".jpg", ".jpeg"];

    [ObservableProperty]
    private ObservableCollection<ImageItemViewModel> _images = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RecompressCommand))]
    private bool _hasFiles;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddFilesCommand))]
    [NotifyCanExecuteChangedFor(nameof(RecompressCommand))]
    private bool _isCompressing;

    [ObservableProperty]
    private string _windowTitle = "SquashPicture";

    [ObservableProperty]
    private bool _replaceOriginalFiles;

    private static readonly string OutputFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
        "SquashPicture_Output");

    public MainWindowViewModel(IFileDialogService fileDialogService, ICompressionService compressionService)
    {
        _fileDialogService = fileDialogService;
        _compressionService = compressionService;
        Images.CollectionChanged += (_, _) => UpdateState();
    }

    [RelayCommand(CanExecute = nameof(CanAddFiles))]
    private async Task AddFilesAsync()
    {
        var files = await _fileDialogService.OpenFileDialogAsync(
            "Select Images",
            null,
            SupportedExtensions);

        if (files is null)
            return;

        AddFiles(files);
        await ProcessQueuedImagesAsync();
    }

    private bool CanAddFiles() => !IsCompressing;

    [RelayCommand(CanExecute = nameof(CanRecompress))]
    private async Task RecompressAsync()
    {
        foreach (var image in Images)
        {
            image.Status = CompressionStatus.Queued;
            image.CompressedSize = 0;
            image.ErrorMessage = null;
            image.IsRecompression = true;
        }

        await ProcessQueuedImagesAsync();
    }

    private bool CanRecompress() => HasFiles && !IsCompressing;

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

    public async Task ProcessDroppedFilesAsync()
    {
        await ProcessQueuedImagesAsync();
    }

    private async Task ProcessQueuedImagesAsync()
    {
        var queuedImages = Images.Where(i => i.Status == CompressionStatus.Queued).ToList();
        if (queuedImages.Count == 0)
            return;

        IsCompressing = true;
        _compressionCts = new CancellationTokenSource();

        try
        {
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = _compressionCts.Token
            };

            await Parallel.ForEachAsync(queuedImages, options, async (imageVm, ct) =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                    imageVm.Status = CompressionStatus.Compressing);

                string? outputPath = null;
                if (!ReplaceOriginalFiles)
                {
                    if (!Directory.Exists(OutputFolder))
                    {
                        Directory.CreateDirectory(OutputFolder);
                    }
                    outputPath = Path.Combine(OutputFolder, imageVm.FileName);
                }

                var result = await _compressionService.CompressAsync(imageVm.FullPath, outputPath, null, ct);

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    imageVm.UpdateFromResult(result);
                    if (result.Success)
                    {
                        imageVm.OriginalSize = result.OriginalSize;
                    }
                });
            });
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            IsCompressing = false;
            _compressionCts?.Dispose();
            _compressionCts = null;
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
