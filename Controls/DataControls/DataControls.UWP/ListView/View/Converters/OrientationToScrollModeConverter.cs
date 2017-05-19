using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Converts a value from the <see cref="Windows.UI.Xaml.Controls.Orientation"/> enumeration to a value <see cref="Windows.UI.Xaml.Controls.ScrollMode"/> value.
    /// </summary>
    public class OrientationToScrollModeConverter : IValueConverter
    {
        private const string Horizontal = "Horizontal";
        private const string Vertical = "Vertical";

        /// <summary>
        /// Returns the scroll mode of a scrollbar of the <see cref="RadListView"/> control depending on the
        /// control orientation and the orientation of the scrollbar.
        /// </summary>
        /// <param name="value"><see cref="RadListView.Orientation"/> property.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The scroll bar orientation.</param>
        /// <param name="language">The parameter is not used.</param>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var controlOrientation = (Orientation)value;
            var scrollBarOrientation = (string)parameter;

            if (controlOrientation == Orientation.Horizontal)
            {
                if (scrollBarOrientation == Horizontal)
                {
                    return ScrollMode.Enabled;
                }
                else
                {
                    return ScrollMode.Disabled;
                }
            }
            else
            {
                if (scrollBarOrientation == Vertical)
                {
                    return ScrollMode.Enabled;
                }
                else
                {
                    return ScrollMode.Disabled;
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
