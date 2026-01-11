using System.Globalization;

namespace ParcAuto_Web_App.Converters;

public class EditModeTitleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isEditMode)
        {
            return isEditMode ? "Edit" : "Add";
        }
        return "Add";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
