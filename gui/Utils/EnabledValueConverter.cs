using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace wiicapture.gui.Utils
{
    /// <summary>
    /// Convert a boolean value into a visibility value
    /// </summary>
    public class EnabledValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool propertyValue = true;
            bool isReversed = false;
           
            if (values.Length == 2)
            {
                propertyValue = (bool)values[0];               
                isReversed = (bool)values[1];
            }
           
            return (object)(isReversed ? !propertyValue : propertyValue);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
