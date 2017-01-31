using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    public class OrientationToRotateDegreesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var orientation = (Orientation)value;

            if (orientation != null)
            {
                if (orientation == Orientation.Vertical)
                {
                    return 0;
                }
                else
                {
                    return -90;
                }
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
