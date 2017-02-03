using Telerik.Charting;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Defines an axis that treats distinct points as "Categories" rather than "Values".
    /// </summary>
    public class CategoricalAxis : CartesianAxis
    {
        /// <summary>
        /// Identifies the <see cref="GapLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GapLengthProperty =
            DependencyProperty.Register(nameof(GapLength), typeof(double), typeof(CategoricalAxis), new PropertyMetadata(0.3, OnGapLengthChanged));

        /// <summary>
        /// Identifies the <see cref="AutoGroup"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoGroupProperty =
            DependencyProperty.Register(nameof(AutoGroup), typeof(bool), typeof(CategoricalAxis), new PropertyMetadata(true, OnAutoGroupPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="MajorTickInterval"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorTickIntervalProperty =
            DependencyProperty.Register(nameof(MajorTickInterval), typeof(int), typeof(CategoricalAxis), new PropertyMetadata(1, OnMajorTickIntervalPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="PlotMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlotModeProperty =
            DependencyProperty.Register(nameof(PlotMode), typeof(AxisPlotMode), typeof(CategoricalAxis), new PropertyMetadata(AxisPlotMode.BetweenTicks, OnPlotModePropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoricalAxis"/> class.
        /// </summary>
        public CategoricalAxis()
        {
            this.DefaultStyleKey = typeof(CategoricalAxis);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the axis will perform its own 
        /// grouping logic or it will consider each data point as a new group.
        /// </summary>
        public bool AutoGroup
        {
            get
            {
                return (this.model as CategoricalAxisModel).AutoGroup;
            }
            set
            {
                this.SetValue(AutoGroupProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the step at which ticks are positioned.
        /// </summary>
        public int MajorTickInterval
        {
            get
            {
                return (this.model as CategoricalAxisModel).MajorTickInterval;
            }
            set
            {
                this.SetValue(MajorTickIntervalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the plot mode used to position points along the axis.
        /// </summary>
        public AxisPlotMode PlotMode
        {
            get
            {
                return (this.model as CategoricalAxisModel).PlotMode;
            }
            set
            {
                this.SetValue(PlotModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the gap (in the range [0, 1]) to be applied when calculating each plotted <see cref="CategoricalSeries"/> position.
        /// </summary>
        public double GapLength
        {
            get
            {
                return (this.model as CategoricalAxisModel).GapLength;
            }
            set
            {
                this.SetValue(GapLengthProperty, value);
            }
        }

        internal override AxisModel CreateModel()
        {
            return new CategoricalAxisModel();
        }
    
        private static void OnGapLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CategoricalAxis axis = d as CategoricalAxis;
            (axis.model as CategoricalAxisModel).GapLength = (double)e.NewValue;
        }

        private static void OnAutoGroupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CategoricalAxis axis = d as CategoricalAxis;
            (axis.model as CategoricalAxisModel).AutoGroup = (bool)e.NewValue;
        }

        private static void OnMajorTickIntervalPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CategoricalAxis axis = sender as CategoricalAxis;
            (axis.model as CategoricalAxisModel).MajorTickInterval = (int)args.NewValue;
        }

        private static void OnPlotModePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CategoricalAxis axis = sender as CategoricalAxis;
            (axis.model as CategoricalAxisModel).PlotMode = (AxisPlotMode)args.NewValue;
        }
    }
}