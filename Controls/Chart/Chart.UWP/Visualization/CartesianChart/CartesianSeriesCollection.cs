using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a typed collection that stores <see cref="CartesianSeries"/> instances.
    /// </summary>
    public sealed class CartesianSeriesCollection : PresenterCollection<CartesianSeries>
    {
        internal CartesianSeriesCollection(RadChartBase owner)
            : base(owner)
        {
        }
    }
}
