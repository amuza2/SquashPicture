using CommunityToolkit.Mvvm.ComponentModel;
using SquashPicture.Models;

namespace SquashPicture.ViewModels;

public partial class ImageItemViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private string _fullPath = string.Empty;

    [ObservableProperty]
    private string _extension = string.Empty;

    [ObservableProperty]
    private long _originalSize;

    [ObservableProperty]
    private long _compressedSize;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusIcon))]
    private CompressionStatus _status = CompressionStatus.Queued;

    [ObservableProperty]
    private string? _errorMessage;

    public string StatusIcon => Status switch
    {
        CompressionStatus.Queued => "📋",
        CompressionStatus.Compressing => "⏳",
        CompressionStatus.Completed => "✓",
        CompressionStatus.Error => "❌",
        _ => ""
    };

    [ObservableProperty]
    private bool _isRecompression;

    public double CompressionRatio => OriginalSize > 0
        ? (1 - (double)CompressedSize / OriginalSize) * 100
        : 0;

    public ImageItemViewModel()
    {
    }

    public ImageItemViewModel(string filePath)
    {
        FullPath = filePath;
        FileName = Path.GetFileName(filePath);
        Extension = Path.GetExtension(filePath);
        
        if (File.Exists(filePath))
        {
            OriginalSize = new FileInfo(filePath).Length;
        }
    }

    public void UpdateFromResult(CompressionResult result)
    {
        if (result.Success)
        {
            CompressedSize = result.CompressedSize;
            Status = CompressionStatus.Completed;
            OnPropertyChanged(nameof(CompressionRatio));
        }
        else
        {
            Status = CompressionStatus.Error;
            ErrorMessage = result.ErrorMessage;
        }
    }

    public ImageFile ToModel() => new()
    {
        FullPath = FullPath,
        FileName = FileName,
        Extension = Extension,
        OriginalSize = OriginalSize,
        CompressedSize = CompressedSize,
        Status = Status,
        IsRecompression = IsRecompression,
        ErrorMessage = ErrorMessage
    };
}
