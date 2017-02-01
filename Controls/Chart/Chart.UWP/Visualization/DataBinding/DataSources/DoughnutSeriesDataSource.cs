using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class DoughnutSeriesDataSource : PieSeriesDataSource
    {
        protected override DataPoint CreateDataPoint()
        {
            return new DoughnutDataPoint();
        }
    }
}
