using Telerik.Charting;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents an <see cref="Axis"/> that recognizes DateTime values and organizes all the plotted points in chronologically sorted categories.
    /// </summary>
    public class DateTimeCategoricalAxis : CategoricalAxis
    {
        /// <summary>
        /// Identifies the <see cref="DateTimeComponent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DateTimeComponentProperty =
            DependencyProperty.Register(nameof(DateTimeComponent), typeof(DateTimeComponent), typeof(DateTimeCategoricalAxis), new PropertyMetadata(DateTimeComponent.Ticks, OnDateTimeComponentPropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeCategoricalAxis"/> class.
        /// </summary>
        public DateTimeCategoricalAxis()
        {
            this.DefaultStyleKey = typeof(DateTimeCategoricalAxis);
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTimeComponent"/> used to determine how data points will be grouped.
        /// </summary>
        public DateTimeComponent DateTimeComponent
        {
            get
            {
                return (this.model as DateTimeCategoricalAxisModel).DateTimeComponent;
            }
            set
            {
                this.SetValue(DateTimeComponentProperty, value);
            }
        }

        internal override AxisModel CreateModel()
        {
            return new DateTimeCategoricalAxisModel();
        }

        private static void OnDateTimeComponentPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            DateTimeCategoricalAxis axis = sender as DateTimeCategoricalAxis;
            (axis.model as DateTimeCategoricalAxisModel).DateTimeComponent = (DateTimeComponent)args.NewValue;
        }
    }
}
