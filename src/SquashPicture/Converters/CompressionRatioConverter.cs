using System.Globalization;
using Avalonia.Data.Converters;
using SquashPicture.Models;

namespace SquashPicture.Converters;

public class CompressionRatioConverter : IMultiValueConverter
{
    public static readonly CompressionRatioConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return "-";

        if (values[0] is not CompressionStatus status)
            return "-";

        if (status == CompressionStatus.Queued)
            return "Queued";

        if (status == CompressionStatus.Compressing)
            return "Compressing...";

        if (status == CompressionStatus.Error)
            return "Error";

        if (values[1] is not double ratio)
            return "-";

        return $"{ratio:0.0}%";
    }
}
