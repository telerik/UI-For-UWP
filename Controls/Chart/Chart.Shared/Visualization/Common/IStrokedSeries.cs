using System;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Identifies a <see cref="ChartSeries"/> instance, which interior may be outlined.
    /// </summary>
    public interface IStrokedSeries
    {
        /// <summary>
        /// Gets the <see cref="Brush"/> instance that defines the stroke of the series.
        /// </summary>
        Brush Stroke
        {
            get;
        }

        /// <summary>
        /// Gets the thickness of the outline shape.
        /// </summary>
        double StrokeThickness
        {
            get;
        }

        /// <summary>
        /// Gets a collection of <see cref="T:System.Double" /> values that indicates the pattern of dashes and gaps that is used to outline stroked series.
        /// </summary>
        DoubleCollection StrokeDashArray
        {
            get;
        }

        /// <summary>
        /// Gets a <see cref="PenLineJoin"/> enumeration value that specifies the type of join that is used at the vertices of a stroked series.
        /// </summary>
        PenLineJoin StrokeLineJoin
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Stroke"/> property has been set locally.
        /// </summary>
        bool IsStrokeSetLocally
        {
            get;
        }
    }
}
