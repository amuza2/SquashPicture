using System.Diagnostics;
using ImageMagick;
using SquashPicture.Models;

namespace SquashPicture.Compressors;

public class JpegCompressor : IImageCompressor
{
    public string[] SupportedExtensions => [".jpg", ".jpeg"];

    public async Task<CompressionResult> CompressAsync(
        string inputPath,
        string? outputPath = null,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var originalSize = new FileInfo(inputPath).Length;
        var replaceOriginal = string.IsNullOrEmpty(outputPath);
        var targetPath = outputPath ?? inputPath;
        string? backupPath = null;

        try
        {
            if (replaceOriginal)
            {
                backupPath = Path.Combine(
                    Path.GetTempPath(),
                    $"{Guid.NewGuid()}_{Path.GetFileName(inputPath)}");
                File.Copy(inputPath, backupPath, overwrite: true);
            }

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var image = new MagickImage(inputPath);

                image.Strip();

                image.Settings.Interlace = Interlace.Plane;

                var originalQuality = image.Quality;
                image.Quality = originalQuality;

                image.Write(targetPath);

                cancellationToken.ThrowIfCancellationRequested();

                var optimizer = new ImageOptimizer
                {
                    OptimalCompression = true
                };
                optimizer.LosslessCompress(targetPath);

            }, cancellationToken);

            var compressedSize = new FileInfo(targetPath).Length;

            if (replaceOriginal && compressedSize >= originalSize)
            {
                File.Copy(backupPath!, targetPath, overwrite: true);
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
        catch (OperationCanceledException)
        {
            if (replaceOriginal && backupPath != null && File.Exists(backupPath))
            {
                try
                {
                    File.Copy(backupPath, inputPath, overwrite: true);
                }
                catch
                {
                }
            }

            throw;
        }
        catch (Exception ex)
        {
            if (replaceOriginal && backupPath != null && File.Exists(backupPath))
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
