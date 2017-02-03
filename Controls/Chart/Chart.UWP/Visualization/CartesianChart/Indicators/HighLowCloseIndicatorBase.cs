using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Base class for financial indicators that are calculated upon the High and Low values.
    /// </summary>
    public abstract class HighLowCloseIndicatorBase : HighLowIndicatorBase
    {
        /// <summary>
        /// Identifies the <see cref="CloseBinding"/> property.
        /// </summary>
        public static readonly DependencyProperty CloseBindingProperty =
            DependencyProperty.Register(nameof(CloseBinding), typeof(DataPointBinding), typeof(HighLowCloseIndicatorBase), new PropertyMetadata(null, OnCloseBindingChanged));

        /// <summary>
        /// Gets or sets the binding that will be used to fill the High value for the indicator calculations.
        /// </summary>
        public DataPointBinding CloseBinding
        {
            get
            {
                return this.GetValue(CloseBindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(CloseBindingProperty, value);
            }
        }

        private static void OnCloseBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HighLowCloseIndicatorBase presenter = d as HighLowCloseIndicatorBase;
            (presenter.dataSource as HighLowCloseIndicatorDataSourceBase).CloseBinding = e.NewValue as DataPointBinding;
        }
    }
}
