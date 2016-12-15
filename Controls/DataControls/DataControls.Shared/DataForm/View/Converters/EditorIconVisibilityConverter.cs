using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;


namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    public class EditorIconVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter != null && parameter.ToString() == "HasErrors")
            {
                if (value != null)
                {
                   var i = System.Convert.ToInt32(value);
                    return i > 0;
                }
            }     

            if (value != null)
            {
                var displayMode = (EditorIconDisplayMode)value;
                var iconType = parameter.ToString();
                if (iconType == "Label")
                {
                    if (displayMode == EditorIconDisplayMode.All || displayMode == EditorIconDisplayMode.Label)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                else if (iconType == "Error")
                {
                    if (displayMode == EditorIconDisplayMode.All || displayMode == EditorIconDisplayMode.Error)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
            }  

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
