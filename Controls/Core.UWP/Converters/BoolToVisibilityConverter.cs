using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.Core
{
    /// <summary>
    /// Converts boolean values to <see cref="Windows.UI.Xaml.Visibility"/>.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display
        /// in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by
        /// the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isInverted = false;

            if (parameter is string)
            {
                bool.TryParse(parameter.ToString(), out isInverted);
            }

            bool boolValue = (bool)value;

            boolValue = isInverted ? !boolValue : boolValue;

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This
        /// method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay" />
        /// bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by
        /// the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}