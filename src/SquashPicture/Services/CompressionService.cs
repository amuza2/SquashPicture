using SquashPicture.Compressors;
using SquashPicture.Models;
using SquashPicture.Services.Interfaces;

namespace SquashPicture.Services;

public class CompressionService : ICompressionService
{
    private readonly IImageCompressor[] _compressors;

    public CompressionService()
    {
        _compressors =
        [
            new PngCompressor(),
            new JpegCompressor()
        ];
    }

    public async Task<CompressionResult> CompressAsync(
        string filePath,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
        {
            return new CompressionResult
            {
                Success = false,
                ErrorMessage = "File not found"
            };
        }

        var compressor = GetCompressor(filePath);
        if (compressor == null)
        {
            return new CompressionResult
            {
                Success = false,
                ErrorMessage = "Unsupported file format"
            };
        }

        try
        {
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.IsReadOnly)
            {
                return new CompressionResult
                {
                    Success = false,
                    OriginalSize = fileInfo.Length,
                    ErrorMessage = "File is read-only"
                };
            }
        }
        catch (Exception ex)
        {
            return new CompressionResult
            {
                Success = false,
                ErrorMessage = $"Cannot access file: {ex.Message}"
            };
        }

        return await compressor.CompressAsync(filePath, cancellationToken);
    }

    public async Task CompressBatchAsync(
        IEnumerable<ImageFile> files,
        IProgress<(ImageFile file, CompressionResult result)>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var fileList = files.ToList();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            CancellationToken = cancellationToken
        };

        await Parallel.ForEachAsync(fileList, options, async (file, ct) =>
        {
            var result = await CompressAsync(file.FullPath, null, ct);
            progress?.Report((file, result));
        });
    }

    public bool IsSupported(string filePath)
    {
        return GetCompressor(filePath) != null;
    }

    private IImageCompressor? GetCompressor(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        return _compressors.FirstOrDefault(c =>
            c.SupportedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase));
    }
}
