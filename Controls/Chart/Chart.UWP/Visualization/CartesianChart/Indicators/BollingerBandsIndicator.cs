using System.Collections.Generic;
using Telerik.Charting;
using Telerik.Core;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents the Bollinger Bands financial indicator.
    /// </summary>
    public class BollingerBandsIndicator : ValuePeriodIndicatorBase
    {
        /// <summary>
        /// Identifies the <see cref="StandardDeviations"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StandardDeviationsProperty =
            DependencyProperty.Register(nameof(StandardDeviations), typeof(int), typeof(ValuePeriodIndicatorBase), new PropertyMetadata(0, OnStandardDeviationsChanged));

        /// <summary>
        /// Identifies the <see cref="LowerBandStroke"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LowerBandStrokeProperty =
            DependencyProperty.Register(nameof(LowerBandStroke), typeof(Brush), typeof(BollingerBandsIndicator), new PropertyMetadata(null, OnLowerBandStrokeChanged));

        internal LineRenderer lowerBandRenderer;
        internal CategoricalSeriesModel lowerBandModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="BollingerBandsIndicator" /> class.
        /// </summary>
        public BollingerBandsIndicator()
        {
            this.DefaultStyleKey = typeof(BollingerBandsIndicator);

            this.lowerBandModel = new CategoricalStrokedSeriesModel();

            this.lowerBandRenderer = this.CreateRenderer();
            this.lowerBandRenderer.model = this.lowerBandModel;
        }

        /// <summary>
        /// Gets or sets the number of standard deviations used to calculate the indicator values.
        /// </summary>
        /// <value>The number of standard deviations.</value>
        public int StandardDeviations
        {
            get
            {
                return (int)this.GetValue(StandardDeviationsProperty);
            }
            set
            {
                this.SetValue(StandardDeviationsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the lower band stroke.
        /// </summary>
        /// <value>The lower band stroke.</value>
        public Brush LowerBandStroke
        {
            get
            {
                return (Brush)this.GetValue(LowerBandStrokeProperty);
            }
            set
            {
                this.SetValue(LowerBandStrokeProperty, value);
            }
        }

        internal List<Element> Elements
        {
            get
            {
                return new List<Element> { this.model, this.lowerBandModel };
            }
        }

        /// <summary>
        /// Gets the collection of data points associated with the signal line.
        /// </summary>
        internal DataPointCollection<CategoricalDataPoint> LowerBandDataPoints
        {
            get
            {
                return this.lowerBandModel.DataPoints;
            }
        }

        internal ChartSeriesModel LowerBandModel
        {
            get
            {
                return this.lowerBandModel;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return "Bollinger Bands (" + this.Period + ", " + this.StandardDeviations + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new BollingerBandsIndicatorDataSource();
        }

        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);
            
            this.lowerBandRenderer.Render(this.drawWithComposition);

            if (this.drawWithComposition && this.lowerBandRenderer.renderPoints.Count > 2)
            {
                foreach (DataPointSegment dataSegment in ChartSeriesRenderer.GetDataSegments(this.lowerBandRenderer.renderPoints))
                {
                    this.chart.ContainerVisualsFactory.PrepareLineRenderVisual(lineRendererVisual, this.lowerBandRenderer.GetPoints(dataSegment), this.LowerBandStroke, this.StrokeThickness);
                }
            }
        }

        /// <summary>
        /// Occurs when one of the axes of the owning <see cref="RadCartesianChart"/> has been changed.
        /// </summary>
        /// <param name="oldAxis">The old axis.</param>
        /// <param name="newAxis">The new axis.</param>
        internal override void ChartAxisChanged(Axis oldAxis, Axis newAxis)
        {
            base.ChartAxisChanged(oldAxis, newAxis);

            if (oldAxis != null)
            {
                this.lowerBandModel.DetachAxis(oldAxis.model);
            }
            if (newAxis != null)
            {
                this.lowerBandModel.AttachAxis(newAxis.model, newAxis.type);
            }
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.renderSurface != null && !this.drawWithComposition)
            {
                this.renderSurface.Children.Remove(this.lowerBandRenderer.strokeShape);
            }
            else if (this.drawWithComposition)
            {
                this.ContainerVisualRoot.Children.Remove(this.lineRendererVisual);
            }
        }

        /// <summary>
        /// Adds the polyline shape to the visual tree.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            if (applied && !this.drawWithComposition)
            {
                this.renderSurface.Children.Add(this.lowerBandRenderer.strokeShape);
            }
            else if (this.drawWithComposition)
            {
                this.lineRendererVisual = this.chart.ContainerVisualsFactory.CreateContainerVisual(this.Compositor, this.GetType());
                this.ContainerVisualRoot.Children.InsertAtBottom(this.lineRendererVisual);
            }

            return applied;
        }

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.lowerBandModel.AttachAxis(this.model.firstAxis, AxisType.First);
            this.lowerBandModel.AttachAxis(this.model.secondAxis, AxisType.Second);

            foreach (var element in this.Elements)
            {
                element.presenter = this;
            }

            this.chart.chartArea.Series.Add(this.lowerBandModel);
        }

        /// <summary>
        /// Occurs when the presenter has been successfully detached from its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected override void OnDetached(RadChartBase oldChart)
        {
            base.OnDetached(oldChart);

            if (oldChart != null)
            {
                foreach (var element in this.Elements)
                {
                    element.presenter = null;
                }

                if (oldChart.chartArea != null && oldChart.chartArea.Series != null)
                {
                    oldChart.chartArea.Series.Remove(this.lowerBandModel);
                }
            }
            else
            {
                // TODO: consider throwing exception.
            }
        }

        /// <summary>
        /// Called when the StrokeThickness property is changed.
        /// </summary>
        protected override void OnStrokeThicknessChanged(double newValue)
        {
            this.lowerBandRenderer.strokeShape.StrokeThickness = newValue;
        }

        /// <summary>
        /// Called when the StrokeDashArray property is changed.
        /// </summary>
        protected override void OnStrokeDashArrayChanged(DoubleCollection newValue)
        {
            this.lowerBandRenderer.strokeShape.StrokeDashArray = newValue.Clone();
        }

        /// <summary>
        /// Called when the StrokeLineJoin property is changed.
        /// </summary>
        protected override void OnStrokeLineJoinChanged(PenLineJoin newValue)
        {
            this.lowerBandRenderer.strokeShape.StrokeLineJoin = newValue;
        }

        private static void OnStandardDeviationsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BollingerBandsIndicator presenter = d as BollingerBandsIndicator;
            (presenter.dataSource as BollingerBandsIndicatorDataSource).StandardDeviations = (int)e.NewValue;
        }

        private static void OnLowerBandStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BollingerBandsIndicator indicator = d as BollingerBandsIndicator;
            indicator.lowerBandRenderer.strokeShape.Stroke = e.NewValue as Brush;
        }
    }
}
