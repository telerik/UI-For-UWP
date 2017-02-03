using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents the Relative Momentum Index financial indicator.
    /// </summary>
    public class RelativeMomentumIndexIndicator : ValuePeriodIndicatorBase
    {
        /// <summary>
        /// Identifies the <see cref="MomentumPeriod"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MomentumPeriodProperty =
            DependencyProperty.Register(nameof(MomentumPeriod), typeof(int), typeof(RelativeMomentumIndexIndicator), new PropertyMetadata(1, OnMomentumPeriodChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeMomentumIndexIndicator"/> class.
        /// </summary>
        public RelativeMomentumIndexIndicator()
        {
            this.DefaultStyleKey = typeof(RelativeMomentumIndexIndicator);
        }

        /// <summary>
        /// Gets or sets the shift. This is the momentum period.
        /// </summary>
        /// <value>The shift.</value>
        public int MomentumPeriod
        {
            get
            {
                return (int)this.GetValue(MomentumPeriodProperty);
            }
            set
            {
                this.SetValue(MomentumPeriodProperty, value);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        public override string ToString()
        {
            return "Relative Momentum Index Indicator (" + this.Period + ", " + this.MomentumPeriod + ")";
        }

        internal override ChartSeriesDataSource CreateDataSourceInstance()
        {
            return new RelativeMomentumIndexIndicatorDataSource();
        }

        private static void OnMomentumPeriodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RelativeMomentumIndexIndicator presenter = d as RelativeMomentumIndexIndicator;
            (presenter.dataSource as RelativeMomentumIndexIndicatorDataSource).MomentumPeriod = (int)e.NewValue;
        }
    }
}
