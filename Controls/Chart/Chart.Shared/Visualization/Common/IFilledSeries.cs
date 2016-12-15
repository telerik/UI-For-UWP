using System;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Identifies a <see cref="ChartSeries"/> instance, which interior may be filled.
    /// </summary>
    public interface IFilledSeries
    {
        /// <summary>
        /// Gets the <see cref="Brush"/> instance that defines the interior of the series.
        /// </summary>
        Brush Fill
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Fill"/> property has been set locally.
        /// </summary>
        bool IsFillSetLocally
        {
            get;
        }
    }
}
