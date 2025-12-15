using Avalonia.Controls;
using Avalonia.Platform.Storage;
using SquashPicture.Services.Interfaces;

namespace SquashPicture.Services;

public class FileDialogService : IFileDialogService
{
    private readonly Window _window;

    public FileDialogService(Window window)
    {
        _window = window;
    }

    public async Task<IEnumerable<string>?> OpenFileDialogAsync(
        string title,
        string? initialDirectory,
        IEnumerable<string> filters)
    {
        var storageProvider = _window.StorageProvider;

        var fileTypes = new FilePickerFileType("Images")
        {
            Patterns = filters.Select(f => $"*{f}").ToList()
        };

        IStorageFolder? startLocation = null;
        if (!string.IsNullOrEmpty(initialDirectory) && Directory.Exists(initialDirectory))
        {
            startLocation = await storageProvider.TryGetFolderFromPathAsync(initialDirectory);
        }

        var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = true,
            FileTypeFilter = [fileTypes],
            SuggestedStartLocation = startLocation
        });

        if (result.Count == 0)
            return null;

        return result.Select(f => f.Path.LocalPath);
    }
}
