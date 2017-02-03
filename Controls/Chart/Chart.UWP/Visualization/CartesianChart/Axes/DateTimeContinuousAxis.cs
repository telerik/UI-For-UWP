using System;
using Telerik.Charting;
using Telerik.Core;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents an <see cref="Axis"/> which plots points along the actual timeline.
    /// </summary>
    public class DateTimeContinuousAxis : CartesianAxis
    {
        /// <summary>
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(DateTime), typeof(DateTimeContinuousAxis), new PropertyMetadata(DateTime.MinValue, OnMinimumChanged));

        /// <summary>
        /// Identifies the <see cref="Maximum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(DateTime), typeof(DateTimeContinuousAxis), new PropertyMetadata(DateTime.MaxValue, OnMaximumChanged));

        /// <summary>
        /// Identifies the <see cref="MajorStep"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorStepProperty =
            DependencyProperty.Register(nameof(MajorStep), typeof(double), typeof(DateTimeContinuousAxis), new PropertyMetadata(0d, OnMajorStepChanged));

        /// <summary>
        /// Identifies the <see cref="MajorStepUnit"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorStepUnitProperty =
            DependencyProperty.Register(nameof(MajorStepUnit), typeof(TimeInterval), typeof(DateTimeContinuousAxis), new PropertyMetadata(TimeInterval.Year, OnMajorStepUnitChanged));

        /// <summary>
        /// Identifies the <see cref="GapLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GapLengthProperty =
            DependencyProperty.Register(nameof(GapLength), typeof(double), typeof(DateTimeContinuousAxis), new PropertyMetadata(0.3, OnGapLengthChanged));

        /// <summary>
        /// Identifies the <see cref="PlotStretch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlotStretchProperty =
            DependencyProperty.Register(nameof(PlotStretch), typeof(DateTimePlotStretchMode), typeof(DateTimeContinuousAxis), new PropertyMetadata(DateTimePlotStretchMode.TickSlot, OnPlotStretchChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeContinuousAxis"/> class.
        /// </summary>
        public DateTimeContinuousAxis()
        {
            this.DefaultStyleKey = typeof(DateTimeContinuousAxis);
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTimePlotStretchMode"/> value that determines the length of each data point plotted by the axis.
        /// </summary>
        public DateTimePlotStretchMode PlotStretch
        {
            get
            {
                return (DateTimePlotStretchMode)this.GetValue(PlotStretchProperty);
            }
            set
            {
                this.SetValue(PlotStretchProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum ticks that might be displayed on the axis.
        /// This property is useful in some corner cases when ticks may become a really big number.
        /// </summary>
        public int MaximumTicks
        {
            get
            {
                return (this.model as DateTimeContinuousAxisModel).MaximumTicks;
            }
            set
            {
                (this.model as DateTimeContinuousAxisModel).MaximumTicks = value;
            }
        }

        /// <summary>
        /// Gets or sets the gap (in the range [0, 1]) to be applied when calculating each plotted <see cref="ChartSeries"/> position.
        /// </summary>
        public double GapLength
        {
            get
            {
                return (this.model as DateTimeContinuousAxisModel).GapLength;
            }
            set
            {
                this.SetValue(GapLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the user-defined step between two adjacent ticks on the axis. Specify <see cref="TimeSpan.Zero"/> to clear the value.
        /// If not specified, the step will be automatically determined, depending on the smallest difference between any two dates.
        /// </summary>
        public double MajorStep
        {
            get
            {
                return (this.model as DateTimeContinuousAxisModel).MajorStep;
            }
            set
            {
                this.SetValue(MajorStepProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the unit that defines the custom major step of the axis.
        /// If no explicit step is defined, the axis will automatically calculate one, depending on the smallest difference between any two dates.
        /// </summary>
        public TimeInterval MajorStepUnit
        {
            get
            {
                return (this.model as DateTimeContinuousAxisModel).MajorStepUnit;
            }
            set
            {
                this.SetValue(MajorStepUnitProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the custom minimum of the axis.
        /// Specify DateTime.MinValue to clear the property value so that the minimum is auto-generated.
        /// </summary>
        public DateTime Minimum
        {
            get
            {
                return (this.model as DateTimeContinuousAxisModel).Minimum;
            }
            set
            {
                this.SetValue(MinimumProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the custom maximum of the axis.
        /// Specify DateTime.MaxValue to clear the property value so that the maximum is auto-generated.
        /// </summary>
        public DateTime Maximum
        {
            get
            {
                return (this.model as DateTimeContinuousAxisModel).Maximum;
            }
            set
            {
                this.SetValue(MaximumProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mode which determines how points are plotted by this axis.
        /// </summary>
        public AxisPlotMode PlotMode
        {
            get
            {
                return (this.model as DateTimeContinuousAxisModel).PlotMode;
            }
            set
            {
                (this.model as DateTimeContinuousAxisModel).PlotMode = value;
            }
        }

        /// <summary>
        /// Gets the actual range used by the axis to plot data points.
        /// </summary>
        public ValueRange<DateTime> ActualRange
        {
            get
            {
                return (this.model as DateTimeContinuousAxisModel).actualRange;
            }
        }

        internal override AxisModel CreateModel()
        {
            return new DateTimeContinuousAxisModel();
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeContinuousAxis presenter = d as DateTimeContinuousAxis;
            (presenter.model as DateTimeContinuousAxisModel).Minimum = (DateTime)e.NewValue;
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeContinuousAxis presenter = d as DateTimeContinuousAxis;
            (presenter.model as DateTimeContinuousAxisModel).Maximum = (DateTime)e.NewValue;
        }

        private static void OnMajorStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeContinuousAxis presenter = d as DateTimeContinuousAxis;
            (presenter.model as DateTimeContinuousAxisModel).MajorStep = (double)e.NewValue;
        }

        private static void OnMajorStepUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeContinuousAxis presenter = d as DateTimeContinuousAxis;
            (presenter.model as DateTimeContinuousAxisModel).MajorStepUnit = (TimeInterval)e.NewValue;
        }

        private static void OnGapLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeContinuousAxis axis = d as DateTimeContinuousAxis;
            (axis.model as DateTimeContinuousAxisModel).GapLength = (double)e.NewValue;
        }

        private static void OnPlotStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimeContinuousAxis axis = d as DateTimeContinuousAxis;
            (axis.model as DateTimeContinuousAxisModel).PlotStretch = (DateTimePlotStretchMode)e.NewValue;
        }
    }
}