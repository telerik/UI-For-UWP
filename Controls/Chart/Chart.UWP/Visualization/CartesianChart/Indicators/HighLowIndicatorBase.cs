using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Base class for financial indicators that are calculated upon the High and Low values.
    /// </summary>
    public abstract class HighLowIndicatorBase : LineIndicatorBase
    {
        /// <summary>
        /// Identifies the <see cref="HighBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty HighBindingProperty =
            DependencyProperty.Register(nameof(HighBinding), typeof(DataPointBinding), typeof(HighLowIndicatorBase), new PropertyMetadata(null, OnHighBindingChanged));

        /// <summary>
        /// Identifies the <see cref="LowBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty LowBindingProperty =
            DependencyProperty.Register(nameof(LowBinding), typeof(DataPointBinding), typeof(HighLowIndicatorBase), new PropertyMetadata(null, OnLowBindingChanged));

        /// <summary>
        /// Gets or sets the binding that will be used to fill the High value for the indicator calculations.
        /// </summary>
        public DataPointBinding HighBinding
        {
            get
            {
                return this.GetValue(HighBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(HighBindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the binding that will be used to fill the Low value for the indicator calculations.
        /// </summary>
        public DataPointBinding LowBinding
        {
            get
            {
                return this.GetValue(LowBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(LowBindingProperty, value);
            }
        }

        private static void OnHighBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HighLowIndicatorBase presenter = d as HighLowIndicatorBase;
            (presenter.dataSource as HighLowIndicatorDataSourceBase).HighBinding = e.NewValue as DataPointBinding;
        }

        private static void OnLowBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HighLowIndicatorBase presenter = d as HighLowIndicatorBase;
            (presenter.dataSource as HighLowIndicatorDataSourceBase).LowBinding = e.NewValue as DataPointBinding;
        }
    }
}
