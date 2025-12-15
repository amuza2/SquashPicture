using SquashPicture.Models;

namespace SquashPicture.Services.Interfaces;

public interface ICompressionService
{
    Task<CompressionResult> CompressAsync(
        string filePath,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default);

    Task CompressBatchAsync(
        IEnumerable<ImageFile> files,
        IProgress<(ImageFile file, CompressionResult result)>? progress = null,
        CancellationToken cancellationToken = default);

    bool IsSupported(string filePath);
}
