using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Represents a custom <see cref="IValueConverter"/> that is created to set the alignment of the item action controls of the <see cref="RadListView"/>
    /// depending on the orientation of the control.
    /// </summary>
    public class OrientationToVerticalAlignmentConverter : IValueConverter
    {
        private const string First = "First";
        private const string Second = "Second";

        /// <summary>
        /// Takes <see cref="Windows.UI.Xaml.Controls.Orientation"/> value and returns <see cref="Windows.UI.Xaml.VerticalAlignment"/> value that
        /// specifies the alignment of the corresponding action content.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="targetType">The type of the context.</param>
        /// <param name="parameter">Specifies which action button is being positioned. The available values are: "First" or "Second".</param>
        /// <param name="language">The language parameter.</param>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var controlOrientation = (Orientation)value;

            string type = (string)parameter;

            if (controlOrientation == Orientation.Horizontal)
            {
                if (type == First)
                {
                    return VerticalAlignment.Top;
                }
                else
                {
                    return VerticalAlignment.Bottom;
                }
            }
            else
            {
                return VerticalAlignment.Stretch;
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
