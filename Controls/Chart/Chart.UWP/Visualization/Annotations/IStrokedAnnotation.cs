using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal interface IStrokedAnnotation
    {
        /// <summary>
        /// Gets the <see cref="Brush"/> instance that defines the stroke of the annotation.
        /// </summary>
        Brush Stroke
        {
            get;
        }

        /// <summary>
        /// Gets the thickness of the outline of the annotation.
        /// </summary>
        double StrokeThickness
        {
            get;
        }

        /// <summary>
        /// Gets a collection of <see cref="T:System.Double" /> values that indicates the pattern of dashes and gaps that is used to outline stroked annotations.
        /// </summary>
        DoubleCollection StrokeDashArray
        {
            get;
        }
    }
}
