using System;
using Telerik.Charting;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents an axis that uses the Logarithm function to calculate the values of the plotted points.
    /// </summary>
    public class LogarithmicAxis : NumericalAxis
    {
        /// <summary>
        /// Identifies the <see cref="ExponentStep"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExponentStepProperty = 
            DependencyProperty.Register(nameof(ExponentStep), typeof(double), typeof(LogarithmicAxis), new PropertyMetadata(10d, OnExponentStepChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="LogarithmicAxis"/> class.
        /// </summary>
        public LogarithmicAxis()
        {
            this.DefaultStyleKey = typeof(LogarithmicAxis);
        }

        /// <summary>
        /// Gets or sets the base of the logarithm used for normalizing data points' values.
        /// </summary>
        public double LogarithmBase
        {
            get
            {
                return (this.model as LogarithmicAxisModel).LogarithmBase;
            }
            set
            {
                (this.model as LogarithmicAxisModel).LogarithmBase = value;
            }
        }

        /// <summary>
        /// Gets or sets the exponent step between each axis tick.
        /// By default the axis itself will calculate the exponent step, depending on the plotted data points.
        /// </summary>
        /// <remarks> You can reset this property by setting it to 0 to restore the default behavior.</remarks>
        public double ExponentStep
        {
            get
            {
                return (this.model as LogarithmicAxisModel).MajorStep;
            }
            set
            {
                this.SetValue(ExponentStepProperty, value);
            }
        }

        internal override AxisModel CreateModel()
        {
            return new LogarithmicAxisModel();
        }

        private static void OnExponentStepChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            NumericalAxis presenter = target as NumericalAxis;
            (presenter.model as LogarithmicAxisModel).MajorStep = (double)args.NewValue;
        }
    }
}
