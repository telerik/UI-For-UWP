using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    /// <summary>
    /// Value converter that converts a <see cref="GroupPanelPosition"/> value to a <see cref="VerticalAlignment"/>.
    /// </summary>
    public class GroupPositionToAlignmentConverter : IValueConverter
    {
        /// <summary>
        /// Convert a <see cref="GroupPanelPosition"/> value to a <see cref="VerticalAlignment"/>.
        /// </summary>
        /// <param name="value">The source string being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">Optional parameter. Not used.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The string corresponding to the resource name.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var position = (GroupPanelPosition)value;
            
            if (position == GroupPanelPosition.Left)
            {
                return VerticalAlignment.Stretch;
            }

            if (position == GroupPanelPosition.Bottom)
            {
                return VerticalAlignment.Bottom;
            }

            return value;
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
