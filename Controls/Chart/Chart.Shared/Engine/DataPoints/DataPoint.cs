using Telerik.Core;
namespace Telerik.Charting
{
    /// <summary>
    /// Base class for all data points that may be plotted in a chart.
    /// </summary>
    public abstract class DataPoint : Node
    {
        internal static readonly int IsSelectedPropertyKey = PropertyKeys.Register(typeof(DataPoint), "IsSelected");
        internal static readonly int LabelPropertyKey = PropertyKeys.Register(typeof(DataPoint), "Label");

        internal bool isPositive;
        internal bool isSelected;
        internal bool isEmpty;

        internal bool wasSelected; // used by the selection behavior to determine whether to toggle the selection of the data point
        internal RadSize desiredSize;
        internal object label;
        internal object dataItem; // valid when the datapoint is created for a data-bound series

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPoint"/> class.
        /// </summary>
        protected DataPoint()
        {
            this.desiredSize = RadSize.Invalid;
            this.isEmpty = true;

            this.TrackPropertyChanged = true;
        }

        /// <summary>
        /// Gets or sets the label associated with this point.
        /// </summary>
        public object Label
        {
            get
            {
                if (this.label != null)
                {
                    return this.label;
                }

                return this.GetDefaultLabel();
            }
            set
            {
                this.SetValue(LabelPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets the object instance that represents the data item associated with this point. Valid when the owning ChartSeries is data-bound.
        /// </summary>
        public object DataItem
        {
            get
            {
                return this.dataItem;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data point is currently in a "Selected" state.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.SetValue(IsSelectedPropertyKey, value);
            }
        }

        internal static bool CheckIsEmpty(double value)
        {
            return double.IsNaN(value);
        }

        internal RadSize Measure()
        {
            if (this.desiredSize == RadSize.Invalid)
            {
                this.desiredSize = this.Presenter.MeasureContent(this, this);
            }

            return this.desiredSize;
        }

        /// <summary>
        /// Gets the object that may be displayed for this data point by the chart tooltip.
        /// </summary>
        internal virtual object GetTooltipValue()
        {
            return null;
        }

        internal virtual object GetValueForAxis(AxisModel axis)
        {
            return null;
        }

        internal virtual void SetValueFromAxis(AxisModel axis, object value)
        {
        }

        internal virtual object GetDefaultLabel()
        {
            return null;
        }

        internal virtual RadRect GetPosition()
        {
            return this.LayoutSlot;
        }

        internal virtual bool ContainsPosition(double x, double y)
        {
            return this.layoutSlot.Contains(x, y);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Method is internal")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            if (e.Key == LabelPropertyKey)
            {
                this.label = e.NewValue;
            }
            else if (e.Key == IsSelectedPropertyKey)
            {
                this.isSelected = (bool)e.NewValue;
                this.NotifySelectionChanged();
            }

            base.OnPropertyChanged(e);
        }

        internal override void UnloadCore()
        {
            base.UnloadCore();

            // TODO: Is simply removing object reference enough? Test carefully for possible leaks.
            this.dataItem = null;
        }

        private void NotifySelectionChanged()
        {
            if (!this.IsTreeLoaded)
            {
                return;
            }

            IChartSeries series = this.Presenter as IChartSeries;
            if (series != null)
            {
                series.OnDataPointIsSelectedChanged(this);
            }
        }
    }
}
