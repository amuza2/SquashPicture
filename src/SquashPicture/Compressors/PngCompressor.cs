using System.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SquashPicture.Models;

namespace SquashPicture.Compressors;

public class PngCompressor : IImageCompressor
{
    public string[] SupportedExtensions => [".png"];

    public async Task<CompressionResult> CompressAsync(
        string inputPath,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var originalSize = new FileInfo(inputPath).Length;
        string? backupPath = null;

        try
        {
            backupPath = Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid()}_{Path.GetFileName(inputPath)}");

            File.Copy(inputPath, backupPath, overwrite: true);

            using var image = await Image.LoadAsync(inputPath, cancellationToken);

            image.Metadata.ExifProfile = null;
            image.Metadata.IccProfile = null;
            image.Metadata.IptcProfile = null;
            image.Metadata.XmpProfile = null;

            var encoder = new PngEncoder
            {
                CompressionLevel = PngCompressionLevel.BestCompression,
                FilterMethod = PngFilterMethod.Adaptive,
                SkipMetadata = true
            };

            await image.SaveAsPngAsync(inputPath, encoder, cancellationToken);

            var compressedSize = new FileInfo(inputPath).Length;

            if (compressedSize >= originalSize)
            {
                File.Copy(backupPath, inputPath, overwrite: true);
                compressedSize = originalSize;
            }

            stopwatch.Stop();

            return new CompressionResult
            {
                Success = true,
                OriginalSize = originalSize,
                CompressedSize = compressedSize,
                Duration = stopwatch.Elapsed
            };
        }
        catch (Exception ex)
        {
            if (backupPath != null && File.Exists(backupPath))
            {
                try
                {
                    File.Copy(backupPath, inputPath, overwrite: true);
                }
                catch
                {
                }
            }

            stopwatch.Stop();

            return new CompressionResult
            {
                Success = false,
                OriginalSize = originalSize,
                CompressedSize = 0,
                ErrorMessage = ex.Message,
                Duration = stopwatch.Elapsed
            };
        }
        finally
        {
            if (backupPath != null && File.Exists(backupPath))
            {
                try
                {
                    File.Delete(backupPath);
                }
                catch
                {
                }
            }
        }
    }
}
