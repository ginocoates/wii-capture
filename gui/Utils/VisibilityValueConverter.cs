using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace wiicapture.gui.Utils
{
    /// <summary>
    /// Convert a boolean value into a visibility value
    /// </summary>
    public class VisibilityValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length < 3 || values.Length > 4 || values.Any(v => v == DependencyProperty.UnsetValue)) return Visibility.Visible;

            bool propertyValue1;
            bool propertyValue2;
            bool isReversed;
            bool collapse;

            if (values.Length == 3)
            {
                propertyValue1 = (bool)values[0];
                propertyValue2 = true;
                isReversed = (bool)values[1];
                collapse = (bool)values[2];
            }
            else
            {

                propertyValue1 = (bool)values[0];
                propertyValue2 = (bool)values[1];
                isReversed = (bool)values[2];
                collapse = (bool)values[3];
            }

            var hideType = collapse ? Visibility.Collapsed : Visibility.Hidden;

            if (isReversed)
            {
                return (object)(propertyValue1 && propertyValue2 ? hideType : Visibility.Visible);
            }

            return (object)(propertyValue1 && propertyValue2 ? Visibility.Visible : hideType);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
