using System;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class PieSeriesDataSource : SingleValuePointDataSource
    {
        protected override DataPoint CreateDataPoint()
        {
            return new PieDataPoint();
        }
    }
}
