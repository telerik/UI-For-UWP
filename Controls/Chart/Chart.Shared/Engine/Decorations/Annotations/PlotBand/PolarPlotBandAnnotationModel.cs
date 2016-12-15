using System;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.Charting
{
    internal class PolarPlotBandAnnotationModel : PlotBandAnnotationModel
    {
        internal RadCircle circle1, circle2;

        protected override RadRect ArrangeCore(RadRect rect)
        {
            double radius = rect.Width / 2;
            RadPoint center = rect.Center;
            double normalizedValue1, normalizedValue2;

            var annotationPresenter = this.presenter as PolarAxisPlotBandAnnotation;

            NumericalAxisPlotInfo polarPlot1 = this.firstPlotInfo as NumericalAxisPlotInfo;
            NumericalAxisPlotInfo polarPlot2 = this.secondPlotInfo as NumericalAxisPlotInfo;
            normalizedValue1 = polarPlot1.NormalizedValue;
            normalizedValue2 = polarPlot2.NormalizedValue;

            if (annotationPresenter != null && annotationPresenter.ClipToPlotArea)
            {
                if (normalizedValue1 > 1 && normalizedValue2 > 1)
                {
                    // annotation should not be visualized
                    this.circle1 = new RadCircle();
                    this.circle2 = new RadCircle(); 
                    return new RadRect();
                }
                else
                {
                    normalizedValue1 = Math.Min(1, polarPlot1.NormalizedValue);
                    normalizedValue2 = Math.Min(1, polarPlot2.NormalizedValue);
                }
            }

            this.circle1 = new RadCircle(center, normalizedValue1 * radius);
            this.circle2 = new RadCircle(center, normalizedValue2 * radius);

            return rect;
        }
    }
}