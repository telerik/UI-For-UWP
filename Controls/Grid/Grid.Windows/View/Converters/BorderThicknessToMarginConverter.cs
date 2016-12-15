using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    public class BorderThicknessToMarginConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var margin = (Thickness)value;
            if(margin != null)
            {
                return new Thickness(0, 0, -margin.Right, 0);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
