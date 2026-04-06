using System.Globalization;

namespace AgenteINV.Converters;

public class StockToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var stock = value switch
        {
            int cantidad => cantidad,
            _ => 0
        };

        return stock == 0 ? Colors.Red : Colors.Green;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
