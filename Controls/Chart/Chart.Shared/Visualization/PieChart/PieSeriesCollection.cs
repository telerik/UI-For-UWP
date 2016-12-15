using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a typed collection that stores <see cref="PieSeries"/> instances.
    /// </summary>
    public sealed class PieSeriesCollection : PresenterCollection<PieSeries>
    {
        internal PieSeriesCollection(RadChartBase owner)
            : base(owner)
        {
        }
    }
}
