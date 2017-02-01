using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a typed collection that stores <see cref="PolarSeries"/> instances.
    /// </summary>
    public sealed class PolarSeriesCollection : PresenterCollection<PolarSeries>
    {
        internal PolarSeriesCollection(RadChartBase owner)
            : base(owner)
        {
        }
    }
}
