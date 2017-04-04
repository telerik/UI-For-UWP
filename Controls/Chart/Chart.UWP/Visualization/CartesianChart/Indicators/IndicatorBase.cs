using System;
using Telerik.Charting;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a financial indicator, whose value depends on the values of DataPoints in financial series.
    /// </summary>
    public abstract class IndicatorBase : ChartSeries
    {
        /// <summary>
        /// Identifies the <see cref="CategoryBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty CategoryBindingProperty =
            DependencyProperty.Register(nameof(CategoryBinding), typeof(DataPointBinding), typeof(IndicatorBase), new PropertyMetadata(null, OnCategoryBindingChanged));

        internal CategoricalSeriesModel model;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndicatorBase" /> class.
        /// </summary>
        internal IndicatorBase()
        {
            this.model = new CategoricalStrokedSeriesModel();
        }

        /// <summary>
        /// Gets the collection of data points associated with the indicator.
        /// </summary>
        public DataPointCollection<CategoricalDataPoint> DataPoints
        {
            get
            {
                return (this.Model as CategoricalSeriesModel).DataPoints;
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the <see cref="CategoricalDataPointBase.Category"/> member of the contained data points.
        /// </summary>
        public DataPointBinding CategoryBinding
        {
            get
            {
                return this.GetValue(CategoryBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(CategoryBindingProperty, value);
            }
        }

        internal override ChartSeriesModel Model
        {
            get
            {
                return this.model;
            }
        }

        internal override int DefaultZIndex
        {
            get
            {
                return RadChartBase.IndicatorZIndex + this.Model.Index;
            }
        }

        /// <summary>
        /// Creates the concrete data source for this instance.
        /// </summary>
        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new CategoricalSeriesDataSource();
        }

        internal override void OnPlotOriginChanged(ChartLayoutContext context)
        {
            // TODO: Possible optimizations
            this.UpdateUICore(context);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new IndicatorBaseAutomationPeer(this);
        }

        /// <summary>
        /// Occurs when one of the axes of the owning <see cref="RadCartesianChart"/> has been changed.
        /// </summary>
        /// <param name="oldAxis">The old axis.</param>
        /// <param name="newAxis">The new axis.</param>
        internal virtual void ChartAxisChanged(Axis oldAxis, Axis newAxis)
        {
            if (oldAxis != null)
            {
                this.model.DetachAxis(oldAxis.model);
            }
            if (newAxis != null)
            {
                this.model.AttachAxis(newAxis.model, newAxis.type);
            }
        }

        /// <summary>
        /// Occurs when the presenter has been successfully attached to its owning <see cref="RadChartBase" /> instance.
        /// </summary>
        protected override void OnAttached()
        {
            RadCartesianChart chart = this.chart as RadCartesianChart;
            if (chart.HorizontalAxis != null)
            {
                this.model.AttachAxis(chart.HorizontalAxis.model, AxisType.First);
            }
            if (chart.VerticalAxis != null)
            {
                this.model.AttachAxis(chart.VerticalAxis.model, AxisType.Second);
            }

            base.OnAttached();
        }

        /// <summary>
        /// Occurs when the presenter has been successfully detached from its owning <see cref="RadChartBase" /> instance.
        /// </summary>
        protected override void OnDetached(RadChartBase oldChart)
        {
            RadCartesianChart chart = oldChart as RadCartesianChart;
            this.model.DetachAxis(chart.HorizontalAxis.model);
            this.model.DetachAxis(chart.VerticalAxis.model);

            base.OnDetached(oldChart);
        }

        private static void OnCategoryBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndicatorBase presenter = d as IndicatorBase;
            (presenter.dataSource as CategoricalSeriesDataSourceBase).CategoryBinding = e.NewValue as DataPointBinding;
        }
    }
}
