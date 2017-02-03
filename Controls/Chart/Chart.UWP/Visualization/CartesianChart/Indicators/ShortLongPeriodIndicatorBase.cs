using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Base class for financial indicators, which use <see cref="ShortPeriod"/> and <see cref="LongPeriod"/> properties to define their values.
    /// </summary>
    public abstract class ShortLongPeriodIndicatorBase : ValueIndicatorBase
    {
        /// <summary>
        /// Identifies the <see cref="LongPeriod"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LongPeriodProperty = 
            DependencyProperty.Register(nameof(LongPeriod), typeof(int), typeof(ShortLongPeriodIndicatorBase), new PropertyMetadata(0, OnLongPeriodChanged));

        /// <summary>
        /// Identifies the <see cref="ShortPeriod"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShortPeriodProperty = 
            DependencyProperty.Register(nameof(ShortPeriod), typeof(int), typeof(ShortLongPeriodIndicatorBase), new PropertyMetadata(0, OnShortPeriodChanged));

        /// <summary>
        /// Gets or sets the indicator long period.
        /// </summary>
        /// <value>The long period.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long")]
        public int LongPeriod
        {
            get
            {
                return (int)this.GetValue(LongPeriodProperty);
            }
            set
            {
                this.SetValue(LongPeriodProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the indicator short period.
        /// </summary>
        /// <value>The short period.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "short")]
        public int ShortPeriod
        {
            get
            {
                return (int)this.GetValue(ShortPeriodProperty);
            }
            set
            {
                this.SetValue(ShortPeriodProperty, value);
            }
        }

        private static void OnLongPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShortLongPeriodIndicatorBase presenter = d as ShortLongPeriodIndicatorBase;
            (presenter.dataSource as ShortLongPeriodIndicatorDataSourceBase).LongPeriod = (int)e.NewValue;
        }

        private static void OnShortPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ShortLongPeriodIndicatorBase presenter = d as ShortLongPeriodIndicatorBase;
            (presenter.dataSource as ShortLongPeriodIndicatorDataSourceBase).ShortPeriod = (int)e.NewValue;
        }
    }
}
