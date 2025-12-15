using System.Diagnostics;
using ImageMagick;
using SquashPicture.Models;

namespace SquashPicture.Compressors;

public class JpegCompressor : IImageCompressor
{
    public string[] SupportedExtensions => [".jpg", ".jpeg"];

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

            await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                using var image = new MagickImage(inputPath);

                image.Strip();

                image.Settings.Interlace = Interlace.Plane;

                var originalQuality = image.Quality;
                image.Quality = originalQuality;

                image.Write(inputPath);

                cancellationToken.ThrowIfCancellationRequested();

                var optimizer = new ImageOptimizer
                {
                    OptimalCompression = true
                };
                optimizer.LosslessCompress(inputPath);

            }, cancellationToken);

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
        catch (OperationCanceledException)
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

            throw;
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
