using System;
using Telerik.Charting;
using Telerik.Core;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Base class for all axes that use numbers to plot associated points.
    /// </summary>
    public abstract class NumericalAxis : CartesianAxis
    { 
        /// <summary>
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(NumericalAxis), new PropertyMetadata(double.NegativeInfinity, OnMinimumChanged));

        /// <summary>
        /// Identifies the <see cref="Maximum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(NumericalAxis), new PropertyMetadata(double.PositiveInfinity, OnMaximumChanged));

        /// <summary>
        /// Identifies the <see cref="RangeExtendDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RangeExtendDirectionProperty =
            DependencyProperty.Register(nameof(RangeExtendDirection), typeof(NumericalAxisRangeExtendDirection), typeof(NumericalAxis), new PropertyMetadata(NumericalAxisRangeExtendDirection.Both, OnRangeExtendDirectionChanged));

        /// <summary>
        /// Identifies the <see cref="DesiredTickCount"/> property.
        /// </summary>
        public static readonly DependencyProperty DesiredTickCountProperty =
            DependencyProperty.Register(nameof(DesiredTickCount), typeof(int), typeof(NumericalAxis), new PropertyMetadata(0, OnDesiredTickCountChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericalAxis" /> class.
        /// </summary>
        protected NumericalAxis()
        {
        }

        /// <summary>
        /// Gets or sets the user-defined minimum of the axis. 
        /// By default the axis itself will calculate the minimum, depending on the minimum of the plotted data points.
        /// </summary>
        /// <remarks> You can reset this property by setting it to double.NegativeInfinity to restore the default behavior.</remarks>
        public double Minimum
        {
            get
            {
                return (this.model as NumericalAxisModel).Minimum;
            }
            set
            {
                this.SetValue(MinimumProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the user-defined maximum of the axis. 
        /// By default the axis itself will calculate the maximum, depending on the maximum of the plotted data points.
        /// </summary>
        /// <remarks> You can reset this property by setting it to double.PositiveInfinity to restore the default behavior.</remarks>
        public double Maximum
        {
            get
            {
                return (this.model as NumericalAxisModel).Maximum;
            }
            set
            {
                this.SetValue(MaximumProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies how the auto-range of this axis will be extended so that each data point is visualized in the best possible way.
        /// </summary>
        public NumericalAxisRangeExtendDirection RangeExtendDirection
        {
            get
            {
                return (this.model as NumericalAxisModel).RangeExtendDirection;
            }
            set
            {
                this.SetValue(RangeExtendDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the user-defined number of ticks presented on the axis.
        /// </summary>
        public int DesiredTickCount
        {
            get
            {
                return (this.model as NumericalAxisModel).DesiredTickCount;
            }
            set
            {
                this.SetValue(DesiredTickCountProperty, value);
            }
        }

        /// <summary>
        /// Gets the actual range used by the axis to plot data points.
        /// </summary>
        public ValueRange<double> ActualRange
        {
            get
            {
                return (this.model as NumericalAxisModel).actualRange;
            }
        }
  
        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericalAxis presenter = d as NumericalAxis;
            (presenter.model as NumericalAxisModel).Minimum = (double)e.NewValue;
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericalAxis presenter = d as NumericalAxis;
            (presenter.model as NumericalAxisModel).Maximum = (double)e.NewValue;
        }

        private static void OnDesiredTickCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericalAxis presenter = d as NumericalAxis;
            (presenter.model as NumericalAxisModel).DesiredTickCount = (int)e.NewValue;
        }

        private static void OnRangeExtendDirectionChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            NumericalAxis presenter = target as NumericalAxis;
            (presenter.model as NumericalAxisModel).RangeExtendDirection = (NumericalAxisRangeExtendDirection)args.NewValue;
        }

        private static void OnMajorStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericalAxis presenter = d as NumericalAxis;
            (presenter.model as NumericalAxisModel).MajorStep = (double)e.NewValue;
        }
    }
}