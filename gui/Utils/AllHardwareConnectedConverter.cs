using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace wiicapture.gui.Utils
{
    /// <summary>
    /// Returns true if all of the values passed is are true
    /// </summary>
    public class AllHardwareConnectedConverter : IMultiValueConverter
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
                    "AllHardwareConnectedConverter expects an array of boolean values to be passed",
                    "values");
            }

            // true if all elements are true
            bool enabled = values.ToList().Select(v => (bool)v).All(v => v);

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
