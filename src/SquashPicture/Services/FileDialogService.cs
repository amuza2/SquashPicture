using Avalonia.Controls;
using Avalonia.Platform.Storage;
using SquashPicture.Services.Interfaces;

namespace SquashPicture.Services;

public class FileDialogService : IFileDialogService
{
    private readonly Window _window;
    private readonly ISettingsService _settingsService;

    public FileDialogService(Window window, ISettingsService settingsService)
    {
        _window = window;
        _settingsService = settingsService;
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

        // Use last directory from settings if no initial directory specified
        var startDir = initialDirectory;
        if (string.IsNullOrEmpty(startDir))
        {
            startDir = _settingsService.Settings.LastDirectory;
        }

        IStorageFolder? startLocation = null;
        if (!string.IsNullOrEmpty(startDir) && Directory.Exists(startDir))
        {
            startLocation = await storageProvider.TryGetFolderFromPathAsync(startDir);
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

        var filePaths = result.Select(f => f.Path.LocalPath).ToList();
        
        // Save the directory of the first selected file as last directory
        if (filePaths.Count > 0)
        {
            var firstFilePath = filePaths[0];
            var directory = Path.GetDirectoryName(firstFilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                _settingsService.Settings.LastDirectory = directory;
                _settingsService.Save();
            }
        }

        return filePaths;
    }
}
