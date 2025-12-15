using SquashPicture.Models;

namespace SquashPicture.Compressors;

public interface IImageCompressor
{
    string[] SupportedExtensions { get; }

    Task<CompressionResult> CompressAsync(
        string inputPath,
        CancellationToken cancellationToken = default);
}
