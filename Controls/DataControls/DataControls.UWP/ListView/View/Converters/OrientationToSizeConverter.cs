using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Represents a custom <see cref="IValueConverter"/> that is created to set the height or width of the item action controls of the <see cref="RadListView"/>
    /// depending on the orientation of the control.
    /// </summary>
    public class OrientationToSizeConverter : IValueConverter
    {
        private const double DefaultWidth = 100d;
        private const double DefaultHeight = 50d;

        private const string Width = "Width";
        private const string Height = "Height";

        /// <summary>
        /// Takes <see cref="Windows.UI.Xaml.Controls.Orientation"/> value and returns <see cref="System.Double"/> value that
        /// specifies the height/width of the corresponding action content depending on the <paramref name="parameter"/> value.
        /// </summary>
        /// <param name="value">The orientation of the control.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">Specifies whether the height or the width will be set. The available values are: "Height", "Width".</param>
        /// <param name="language">The parameter is not used.</param>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Orientation controlOrientation = (Orientation)value;
            string type = (string)parameter;

            if (controlOrientation == Orientation.Vertical)
            {
                if (type == Width)
                {
                    return DefaultWidth;
                }
                else
                {
                    return double.NaN;
                }
            }
            else
            {
                if (type == Width)
                {
                    return double.NaN;
                }
                else
                {
                    return DefaultHeight;
                }
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
