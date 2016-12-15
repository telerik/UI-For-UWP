using System;
using System.Collections.Generic;
using System.Text;
using Telerik.Data.Core;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    public class IsEnabledEditorConvetrer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var property = value as EntityProperty;

            if (property != null)
            {
                if (property.Entity != null && property.Entity.IsReadOnly)
                {
                    return false;
                }

                return !property.IsReadOnly;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
