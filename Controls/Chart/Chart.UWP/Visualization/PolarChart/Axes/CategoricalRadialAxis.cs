using Telerik.Charting;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a <see cref="RadialAxis"/> that plots categorical data.
    /// </summary>
    public class CategoricalRadialAxis : RadialAxis
    {
        /// <summary>
        /// Identifies the <see cref="MajorTickInterval"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorTickIntervalProperty =
            DependencyProperty.Register(nameof(MajorTickInterval), typeof(int), typeof(CategoricalRadialAxis), new PropertyMetadata(1, OnMajorTickIntervalPropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoricalRadialAxis" /> class.
        /// </summary>
        public CategoricalRadialAxis()
        {
            this.DefaultStyleKey = typeof(CategoricalRadialAxis);
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

        internal override AxisModel CreateModel()
        {
            return new CategoricalRadialAxisModel();
        }

        private static void OnMajorTickIntervalPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            CategoricalRadialAxis axis = sender as CategoricalRadialAxis;
            (axis.model as CategoricalRadialAxisModel).MajorTickInterval = (int)args.NewValue;
        }
    }
}
