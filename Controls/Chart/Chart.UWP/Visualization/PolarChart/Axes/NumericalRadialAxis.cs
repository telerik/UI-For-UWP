using System;
using Telerik.Charting;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a <see cref="RadialAxis"/> that plots numerical data.
    /// </summary>
    public class NumericalRadialAxis : RadialAxis
    {
        /// <summary>
        /// Identifies the <see cref="MajorStep"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorStepProperty = 
            DependencyProperty.Register(nameof(MajorStep), typeof(double), typeof(NumericalRadialAxis), new PropertyMetadata(0d, OnMajorStepChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericalRadialAxis" /> class.
        /// </summary>
        public NumericalRadialAxis()
        {
            this.DefaultStyleKey = typeof(NumericalRadialAxis);
        }

        /// <summary>
        /// Gets or sets the step of the ticks on the axis ellipse.
        /// </summary>
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
            return new NumericRadialAxisModel();
        }

        private static void OnMajorStepChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            NumericalRadialAxis presenter = target as NumericalRadialAxis;
            (presenter.model as NumericalAxisModel).MajorStep = (double)args.NewValue;
        }
    }
}
