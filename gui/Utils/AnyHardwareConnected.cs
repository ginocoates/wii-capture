using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace wiicapture.gui.Utils
{
    public class AnyHardwareConnectedConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException(
                    "AnyHardwareConnectedConverter expects an array of boolean values to be passed",
                    "values");
            }

            // is there any true value
            bool enabled = values.ToList().Select(v => (bool)v).Any(b => b);

            return (object)enabled;
        }

        public object[] ConvertBack(object value,
                                    Type[] targetTypes,
                                    object parameter,
                                    CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
