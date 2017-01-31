using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    public partial class PolarSeries
    {
        internal void UpdateLegendFill(Brush fill)
        {
            var legendItem = this.LegendItems.FirstOrDefault();
            if (legendItem != null)
            {
                legendItem.Fill = fill ?? legendItem.Stroke;
            }
        }

        internal void UpdateLegendStroke(Brush stroke)
        {
            var legendItem = this.LegendItems.FirstOrDefault();
            if (legendItem != null)
            {
                legendItem.Stroke = stroke;

                if (legendItem.Fill == null)
                {
                    legendItem.Fill = legendItem.Stroke;
                }
            }
        }

        private static void OnLegendTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = (PolarSeries)d;

            var legendItem = series.LegendItems.FirstOrDefault();
            if (legendItem != null)
            {
                legendItem.Title = (string)e.NewValue;
            }
        }

        private static void OnIsVisibleInLegendChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var series = (PolarSeries)d;

            var legendItem = series.LegendItems.FirstOrDefault();
            if (legendItem == null)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                series.Chart.LegendInfosInternal.Add(legendItem);
            }
            else
            {
                series.Chart.LegendInfosInternal.Remove(legendItem);
            }
        }
    }
}