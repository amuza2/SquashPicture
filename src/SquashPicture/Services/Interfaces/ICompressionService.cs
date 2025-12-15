using SquashPicture.Models;

namespace SquashPicture.Services.Interfaces;

public interface ICompressionService
{
    Task<CompressionResult> CompressAsync(
        string filePath,
        string? outputPath = null,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default);

    Task CompressBatchAsync(
        IEnumerable<ImageFile> files,
        string? outputFolder = null,
        IProgress<(ImageFile file, CompressionResult result)>? progress = null,
        CancellationToken cancellationToken = default);

    bool IsSupported(string filePath);
}
