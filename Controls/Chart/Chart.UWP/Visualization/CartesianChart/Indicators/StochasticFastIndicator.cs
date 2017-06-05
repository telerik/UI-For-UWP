using System.Collections.Generic;
using Telerik.Charting;
using Telerik.Core;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents the Stochastic Fast financial indicator.
    /// </summary>
    public class StochasticFastIndicator : HighLowCloseIndicatorBase
    {
        /// <summary>
        /// Identifies the <see cref="MainPeriod"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MainPeriodProperty =
            DependencyProperty.Register(nameof(MainPeriod), typeof(int), typeof(StochasticFastIndicator), new PropertyMetadata(0, OnMainPeriodChanged));

        /// <summary>
        /// Identifies the <see cref="MainPeriod"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SignalPeriodProperty =
            DependencyProperty.Register(nameof(SignalPeriod), typeof(int), typeof(StochasticFastIndicator), new PropertyMetadata(0, OnSignalPeriodChanged));

        /// <summary>
        /// Identifies the <see cref="SignalStroke"/> property.
        /// </summary>
        public static readonly DependencyProperty SignalStrokeProperty =
            DependencyProperty.Register(nameof(SignalStroke), typeof(Brush), typeof(StochasticFastIndicator), new PropertyMetadata(null, OnSignalStrokeChanged));

        internal LineRenderer signalRenderer;
        internal ContainerVisual signalRendererVisual;
        internal CategoricalSeriesModel signalModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="StochasticFastIndicator" /> class.
        /// </summary>
        public StochasticFastIndicator()
        {
            this.DefaultStyleKey = typeof(StochasticFastIndicator);

            this.signalModel = new CategoricalStrokedSeriesModel();

            this.signalRenderer = this.CreateRenderer();
            this.signalRenderer.model = this.signalModel;
        }

        /// <summary>
        /// Gets or sets the indicator main (fast) period.
        /// </summary>
        /// <value>The period.</value>
        public int MainPeriod
        {
            get
            {
                return (int)this.GetValue(MainPeriodProperty);
            }
            set
            {
                this.SetValue(MainPeriodProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the indicator signal (slow) period.
        /// </summary>
        /// <value>The period.</value>
        public int SignalPeriod
        {
            get
            {
                return (int)this.GetValue(SignalPeriodProperty);
            }
            set
            {
                this.SetValue(SignalPeriodProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> instance that defines the stroke of the line.
        /// </summary>
        public Brush SignalStroke
        {
            get
            {
                return this.GetValue(SignalStrokeProperty) as Brush;
            }
            set
            {
                this.SetValue(SignalStrokeProperty, value);
            }
        }

        internal List<Element> Elements
        {
            get
            {
                return new List<Element> { this.model, this.signalModel };
            }
        }

        internal ChartSeriesModel SignalModel
        {
            get
            {
                return this.signalModel;
            }
        }

        /// <summary>
        /// Gets the collection of data points associated with the signal line.
        /// </summary>
        internal DataPointCollection<CategoricalDataPoint> SignalDataPoints
        {
            get
            {
                return this.signalModel.DataPoints;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Stochastic Fast (" + this.MainPeriod + ", " + this.SignalPeriod + ")";
        }

        internal override void UpdateUICore(ChartLayoutContext context)
        {
            base.UpdateUICore(context);
            
            this.signalRenderer.Render(this.drawWithComposition);

            if (this.drawWithComposition && this.signalRenderer.renderPoints.Count > 2)
            {
                foreach (DataPointSegment dataSegment in ChartSeriesRenderer.GetDataSegments(this.signalRenderer.renderPoints))
                {
                    this.chart.ContainerVisualsFactory.PrepareLineRenderVisual(signalRendererVisual, this.signalRenderer.GetPoints(dataSegment), this.SignalStroke, this.StrokeThickness);
                }
            }
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new StochasticFastIndicatorDataSource();
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
                this.signalModel.DetachAxis(oldAxis.model);
            }
            if (newAxis != null)
            {
                this.signalModel.AttachAxis(newAxis.model, newAxis.type);
            }
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            if (this.renderSurface != null && !this.drawWithComposition)
            {
                this.renderSurface.Children.Remove(this.signalRenderer.strokeShape);
            }
            else if (this.drawWithComposition)
            {
                this.ContainerVisualRoot.Children.Remove(this.signalRendererVisual);
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
                this.renderSurface.Children.Add(this.signalRenderer.strokeShape);
            }
            else if (this.drawWithComposition)
            {
                this.signalRendererVisual = this.chart.ContainerVisualsFactory.CreateContainerVisual(this.Compositor, this.GetType());
                this.ContainerVisualRoot.Children.InsertAtBottom(this.signalRendererVisual);
            }


            return applied;
        }

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.signalModel.AttachAxis(this.model.firstAxis, AxisType.First);
            this.signalModel.AttachAxis(this.model.secondAxis, AxisType.Second);

            foreach (Element element in this.Elements)
            {
                element.presenter = this;
            }

            this.chart.chartArea.Series.Add(this.signalModel);
        }

        /// <summary>
        /// Occurs when the presenter has been successfully detached from its owning <see cref="RadChartBase"/> instance.
        /// </summary>
        protected override void OnDetached(RadChartBase oldChart)
        {
            base.OnDetached(oldChart);

            if (oldChart != null)
            {
                foreach (Element element in this.Elements)
                {
                    element.presenter = null;
                }

                if (oldChart.chartArea != null && oldChart.chartArea.Series != null)
                {
                    oldChart.chartArea.Series.Remove(this.signalModel);
                }
            }

            // TODO: consider raising exception if oldChart is invalid.
        }

        /// <summary>
        /// Called when the StrokeThickness property is changed.
        /// </summary>
        protected override void OnStrokeThicknessChanged(double newValue)
        {
            this.signalRenderer.strokeShape.StrokeThickness = newValue;
        }

        /// <summary>
        /// Called when the StrokeDashArray property is changed.
        /// </summary>
        protected override void OnStrokeDashArrayChanged(DoubleCollection newValue)
        {
            this.signalRenderer.strokeShape.StrokeDashArray = newValue.Clone();
        }

        /// <summary>
        /// Called when the StrokeLineJoin property is changed.
        /// </summary>
        protected override void OnStrokeLineJoinChanged(PenLineJoin newValue)
        {
            this.signalRenderer.strokeShape.StrokeLineJoin = newValue;
        }

        private static void OnMainPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StochasticFastIndicator presenter = d as StochasticFastIndicator;
            (presenter.dataSource as StochasticIndicatorDataSourceBase).MainPeriod = (int)e.NewValue;
        }

        private static void OnSignalPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StochasticFastIndicator presenter = d as StochasticFastIndicator;
            (presenter.dataSource as StochasticIndicatorDataSourceBase).SignalPeriod = (int)e.NewValue;
        }

        private static void OnSignalStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StochasticFastIndicator indicator = d as StochasticFastIndicator;
            indicator.signalRenderer.strokeShape.Stroke = e.NewValue as Brush;

            if (indicator.isPaletteApplied)
            {
                indicator.UpdatePalette(true);
            }
        }
    }
}
