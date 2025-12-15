namespace SquashPicture.Services.Interfaces;

public interface IFileDialogService
{
    Task<IEnumerable<string>?> OpenFileDialogAsync(
        string title,
        string? initialDirectory,
        IEnumerable<string> filters);
}
