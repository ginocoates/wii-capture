using System;
using System.Windows.Data;
using System.Windows.Media;

namespace wiicapture.gui.Utils
{
    /// <summary>
    /// Convert the enabled value to a color
    /// </summary>
    public class DeviceStatusColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool enabled = (bool)value;
            Brush brush = new SolidColorBrush(enabled ? Colors.LightGreen : Colors.Red);
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
