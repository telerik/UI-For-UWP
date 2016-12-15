using System;
using Telerik.Core;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Chart.Primitives
{
    /// <summary>
    /// Represents a converter that retrieves a chart palette brush from a <see cref="DataPointInfo"/> object.
    /// </summary>
    public class DataPointInfoToPaletteBrushConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the <see cref="PaletteVisualPart"/> value which will be used to retrieve the palette value from the passed data point.
        /// </summary>
        public PaletteVisualPart PaletteVisualPart { get; set; }

        /// <summary>
        /// Converts a value of type <see cref="DataPointInfo"/> to the corresponding <see cref="Windows.UI.Xaml.Media.Brush"/> from the chart palette.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DataPointInfo context = value as DataPointInfo;
            if (context == null)
            {
                return value;
            }

            Element seriesModel = context.DataPoint.Parent as Element;
            if (seriesModel == null)
            {
                return value;
            }

            var presenter = seriesModel.Presenter as ChartSeries;
            if (presenter == null || presenter.Chart == null)
            {
                return value;
            }

            var palette = presenter.Chart.Palette;
            if (palette != null)
            {
                return palette.GetBrush(presenter.GetPaletteIndexForPoint(context.DataPoint), this.PaletteVisualPart);
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
