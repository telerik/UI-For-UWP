using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a typed collection that stores <see cref="IndicatorBase"/> instances.
    /// </summary>
    public sealed class IndicatorCollection : PresenterCollection<IndicatorBase>
    {
        internal IndicatorCollection(RadChartBase owner)
            : base(owner)
        {
        }
    }
}
