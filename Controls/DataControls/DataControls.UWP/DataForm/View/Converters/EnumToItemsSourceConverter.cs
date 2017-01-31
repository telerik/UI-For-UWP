using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    public class EnumToItemsSourceConverter : IValueConverter
    {    
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                return Enum.GetValues((Type)value);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
}
