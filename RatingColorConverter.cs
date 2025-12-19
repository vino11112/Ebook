using System.Globalization;

namespace Ebook;

public class RatingColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int rating = (int)value;
        int starNumber = int.Parse(parameter.ToString()!);

        return rating >= starNumber ? "#FFD700" : "#CCCCCC";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
