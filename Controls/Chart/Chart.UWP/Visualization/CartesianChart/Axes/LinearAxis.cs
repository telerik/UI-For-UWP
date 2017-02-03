using Telerik.Charting;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a <see cref="NumericalAxis"/> which step is linear.
    /// </summary>
    public class LinearAxis : NumericalAxis
    {
        /// <summary>
        /// Identifies the <see cref="MajorStep"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorStepProperty =
            DependencyProperty.Register(nameof(MajorStep), typeof(double), typeof(LinearAxis), new PropertyMetadata(double.PositiveInfinity, OnMajorStepChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearAxis"/> class.
        /// </summary>
        public LinearAxis()
        {
            this.DefaultStyleKey = typeof(LinearAxis);
        }

        /// <summary>
        /// Gets or sets the major step between each axis tick.
        /// By default the axis itself will calculate the major step, depending on the plotted data points.
        /// </summary>
        /// <remarks> You can reset this property by setting it to 0 to restore the default behavior.</remarks>
        public double MajorStep
        {
            get
            {
                return (this.model as NumericalAxisModel).MajorStep;
            }
            set
            {
                this.SetValue(MajorStepProperty, value);
            }
        }

        internal override AxisModel CreateModel()
        {
            return new LinearAxisModel();
        }

        private static void OnMajorStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericalAxis presenter = d as NumericalAxis;
            (presenter.model as NumericalAxisModel).MajorStep = (double)e.NewValue;
        }
    }
}