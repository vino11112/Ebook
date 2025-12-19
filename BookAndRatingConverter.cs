using System.Globalization;
using Ebook.Models;

namespace Ebook;

public class BookAndRatingConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Book b = value as Book;
        int rating = int.Parse(parameter.ToString()!);

        return (b, rating);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
