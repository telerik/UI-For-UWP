using System;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Primitives.HubTile
{
    /// <summary>
    /// Depending on the provided <see cref="HexOrientation"/> value, returns a string that corresponds to a hexagonal <see cref="Windows.UI.Xaml.Shapes.Path.Data"/>.
    /// </summary>
    public class OrientationToPathDataConverter : IValueConverter
    {
        private const string FlatData = "M0,0.866 L0.5,0 L1.5,0 L2,0.866 L1.5,1.732 L0.5,1.732 z";
        private const string AngledData = "M0,1.5 L0,0.5 L0.866,0 L1.732,0.5 L1.732,1.5 L0.866,2 z";

        /// <summary>
        /// Returns a hexagonal path data depending on the provided <see cref="HexOrientation"/> value.
        /// </summary>
        /// <param name="value">The <see cref="HexOrientation"/> value.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="language">The parameter is not used.</param>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var orientation = (HexOrientation)value;
                return orientation == HexOrientation.Flat ? FlatData : AngledData;
            }

            return null;
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
