using System;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    public class DataOperationsButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var canGroupBy = (bool)value;
            if(canGroupBy)
            {
                return GridLocalizationManager.Instance.GetString("DataOperationsButtonGroup");
            }
            else
            {
                return GridLocalizationManager.Instance.GetString("DataOperationsButtonUngroup");
            }
         }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
