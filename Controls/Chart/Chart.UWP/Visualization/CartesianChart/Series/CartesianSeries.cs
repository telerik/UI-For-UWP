using System.Collections.Generic;
using Telerik.Charting;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents <see cref="ChartSeries"/> that may be visualized by a <see cref="RadCartesianChart"/> instance.
    /// </summary>
    public abstract partial class CartesianSeries : PointTemplateSeries
    {
        /// <summary>
        /// Identifies the <see cref="HorizontalAxis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAxisProperty =
            DependencyProperty.Register(nameof(HorizontalAxis), typeof(CartesianAxis), typeof(CartesianSeries), new PropertyMetadata(null, OnHorizontalAxisChanged));

        /// <summary>
        /// Identifies the <see cref="VerticalAxis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalAxisProperty =
            DependencyProperty.Register(nameof(VerticalAxis), typeof(CartesianAxis), typeof(CartesianSeries), new PropertyMetadata(null, OnVerticalAxisChanged));

        private CartesianAxis horizontalAxisCache;
        private CartesianAxis verticalAxisCache;
        private List<CartesianAxis> unattachedAxes;
        private IPlotAreaElementModelWithAxes axisModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianSeries" /> class.
        /// </summary>
        protected CartesianSeries()
        {
            this.unattachedAxes = new List<CartesianAxis>();
        }

        /// <summary>
        /// Gets or sets the visual <see cref="CartesianAxis"/> instance that will be used to plot points along the horizontal (X) axis.
        /// </summary>
        public CartesianAxis HorizontalAxis
        {
            get
            {
                return this.horizontalAxisCache;
            }
            set
            {
                this.SetValue(HorizontalAxisProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visual <see cref="CartesianAxis"/> instance that will be used to plot points along the vertical (Y) axis.
        /// </summary>
        public CartesianAxis VerticalAxis
        {
            get
            {
                return this.verticalAxisCache;
            }
            set
            {
                this.SetValue(VerticalAxisProperty, value);
            }
        }

        private IPlotAreaElementModelWithAxes AxisModel
        {
            get
            {
                if (this.axisModel == null)
                {
                    this.axisModel = this.Model as IPlotAreaElementModelWithAxes;
                }
                return this.axisModel;
            }
        }

        /// <summary>
        /// Occurs when one of the axes of the owning <see cref="RadCartesianChart"/> has been changed.
        /// </summary>
        /// <param name="oldAxis">The old axis.</param>
        /// <param name="newAxis">The new axis.</param>
        internal void ChartAxisChanged(CartesianAxis oldAxis, CartesianAxis newAxis)
        {
            if (this.horizontalAxisCache == null)
            {
                if (oldAxis != null && oldAxis.type == AxisType.First)
                {
                    this.AxisModel.DetachAxis(oldAxis.model);
                }
                if (newAxis != null && newAxis.type == AxisType.First)
                {
                    this.AxisModel.AttachAxis(newAxis.model, AxisType.First);
                }
            }
            if (this.verticalAxisCache == null)
            {
                if (oldAxis != null && oldAxis.type == AxisType.Second)
                {
                    this.AxisModel.DetachAxis(oldAxis.model);
                }
                if (newAxis != null && newAxis.type == AxisType.Second)
                {
                    this.AxisModel.AttachAxis(newAxis.model, AxisType.Second);
                }
            }
        }

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase" /> instance.
        /// </summary>
        protected override void OnAttached()
        {
            foreach (CartesianAxis axis in this.unattachedAxes)
            {
                this.AddAxisToChart(axis, this.chart);
            }
            this.unattachedAxes.Clear();

            RadCartesianChart cartesianChart = this.chart as RadCartesianChart;
            if (this.horizontalAxisCache == null && cartesianChart.HorizontalAxis != null)
            {
                this.AxisModel.AttachAxis(cartesianChart.HorizontalAxis.model, AxisType.First);
            }
            if (this.verticalAxisCache == null && cartesianChart.VerticalAxis != null)
            {
                this.AxisModel.AttachAxis(cartesianChart.VerticalAxis.model, AxisType.Second);
            }

            base.OnAttached();
        }

        /// <summary>
        /// Occurs when the presenter has been successfully detached from its owning <see cref="RadChartBase" /> instance.
        /// </summary>
        protected override void OnDetached(RadChartBase oldChart)
        {
            if (this.horizontalAxisCache != null)
            {
                this.RemoveAxisFromChart(this.horizontalAxisCache, oldChart, false);
            }
            if (this.verticalAxisCache != null)
            {
                this.RemoveAxisFromChart(this.verticalAxisCache, oldChart, false);
            }

            base.OnDetached(oldChart);
        }

        private static void OnHorizontalAxisChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            CartesianSeries series = target as CartesianSeries;
            CartesianAxis oldAxis = args.OldValue as CartesianAxis;
            CartesianAxis newAxis = args.NewValue as CartesianAxis;

            series.horizontalAxisCache = newAxis;
            if (series.horizontalAxisCache != null)
            {
                series.horizontalAxisCache.type = AxisType.First;
            }

            series.OnAxisChanged(oldAxis, newAxis);
        }

        private static void OnVerticalAxisChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            CartesianSeries series = target as CartesianSeries;
            CartesianAxis oldAxis = args.OldValue as CartesianAxis;
            CartesianAxis newAxis = args.NewValue as CartesianAxis;

            series.verticalAxisCache = newAxis;
            if (series.verticalAxisCache != null)
            {
                series.verticalAxisCache.type = AxisType.Second;
            }

            series.OnAxisChanged(oldAxis, newAxis);
        }

        private void OnAxisChanged(CartesianAxis oldAxis, CartesianAxis newAxis)
        {
            if (oldAxis != null)
            {
                this.RemoveAxisFromChart(oldAxis, this.chart, true);
                this.AxisModel.DetachAxis(oldAxis.model);
            }

            if (newAxis != null)
            {
                this.AddAxisToChart(newAxis, this.chart);
                this.AxisModel.AttachAxis(newAxis.model, newAxis.type);
            }

            if (oldAxis != null && newAxis == null)
            {
                RadCartesianChart cartesianChart = this.chart as RadCartesianChart;
                if (cartesianChart != null)
                {
                    if (oldAxis.type == AxisType.First && cartesianChart.HorizontalAxis != null)
                    {
                        this.AxisModel.AttachAxis(cartesianChart.HorizontalAxis.model, AxisType.First);
                    }
                    else if (oldAxis.type == AxisType.Second && cartesianChart.VerticalAxis != null)
                    {
                        this.AxisModel.AttachAxis(cartesianChart.VerticalAxis.model, AxisType.Second);
                    }
                }
            }
        }

        private void AddAxisToChart(CartesianAxis axis, RadChartBase chart)
        {
            if (chart == null)
            {
                this.unattachedAxes.Add(axis);
            }
            else
            {
                if (axis.chart == null)
                {
                    chart.OnPresenterAdded(axis);
                }
                axis.linkedSeriesCount++;
            }
        }

        private void RemoveAxisFromChart(CartesianAxis axis, RadChartBase chart, bool detachFromSeries)
        {
            if (chart == null)
            {
                this.unattachedAxes.Remove(axis);
            }
            else
            {
                if (axis.linkedSeriesCount == 1)
                {
                    chart.OnPresenterRemoved(axis);
                }
                axis.linkedSeriesCount--;
            }

            if (!detachFromSeries)
            {
                this.unattachedAxes.Add(axis);
            }
        }
    }
}
