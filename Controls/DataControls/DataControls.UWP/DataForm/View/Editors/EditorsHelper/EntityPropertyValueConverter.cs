using System;
using Telerik.Data.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class EntityPropertyValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            object result;
            FrameworkElement frameworkElement = (FrameworkElement)parameter;
            EntityProperty entityProperty = (EntityProperty)frameworkElement.DataContext;

            if (entityProperty != null)
            {
                result = entityProperty.GetConvertValue(value);
            }
            else
            {
                result = value;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            object result;
            FrameworkElement frameworkElement = (FrameworkElement)parameter;
            EntityProperty entityProperty = (EntityProperty)frameworkElement.DataContext;

            if (entityProperty != null)
            {
                result = entityProperty.GetConvertBackValue(value);
            }
            else
            {
                result = value;
            }

            return result;
        }
    }
}