using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using SquashPicture.Models;

namespace SquashPicture.Converters;

public class StatusToColorConverter : IValueConverter
{
    public static readonly StatusToColorConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CompressionStatus status)
            return Brushes.Gray;

        return status switch
        {
            CompressionStatus.Queued => Brushes.Gray,
            CompressionStatus.Compressing => Brushes.Orange,
            CompressionStatus.Completed => Brushes.Green,
            CompressionStatus.Error => Brushes.Red,
            _ => Brushes.Gray
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
