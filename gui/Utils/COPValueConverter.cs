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
    /// <summary>
    /// Convert COP X or Y to a proportion of the canvas taking size of ellipse into account.
    /// Used fro rendering COP on screen
    /// </summary>
    public class COPValueConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            var copData = new COPRenderData(values);

            return (object)copData.IndicatorPosition;
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
