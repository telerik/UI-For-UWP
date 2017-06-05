using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Input.DateTimePickers
{
    /// <summary>
    /// Represents a custom value converter that transforms a <see cref="DateTimePickerDisplayMode"/> value to a <see cref="Visibility"/> value.
    /// </summary>
    public class DisplayModeToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the <see cref="DateTimePickerDisplayMode"/> value that defines the <see cref="Visibility.Visible"/> condition.
        /// </summary>
        public DateTimePickerDisplayMode VisibleDisplayMode { get; set; }

        /// <summary>
        /// Converts <see cref="DateTimePickerDisplayMode"/> values to <see cref="Visibility"/> objects.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTimePickerDisplayMode displayMode = (DateTimePickerDisplayMode)value;
            if (displayMode == this.VisibleDisplayMode)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
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