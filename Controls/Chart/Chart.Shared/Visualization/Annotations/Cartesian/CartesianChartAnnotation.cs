using System;
using Telerik.Core;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// This is the base class for all chart annotations in a <see cref="RadCartesianChart"/>.
    /// </summary>
    public abstract class CartesianChartAnnotation : ChartAnnotation
    {
        internal virtual void ChartAxisChanged(CartesianAxis oldAxis, CartesianAxis newAxis)
        {             
        }

        internal override void UpdateVisibility()
        {
            RadRect plotAreaLayoutSlot = this.chart.chartArea.plotArea.layoutSlot;

            // point is laid-out outside the clip area, skip it from visualization
            if (plotAreaLayoutSlot.IntersectsWith(this.Model.layoutSlot))
            {
                this.SetPresenterVisibility(Visibility.Visible);
            }
            else
            {
                this.SetPresenterVisibility(Visibility.Collapsed);
            }
        }
    }
}