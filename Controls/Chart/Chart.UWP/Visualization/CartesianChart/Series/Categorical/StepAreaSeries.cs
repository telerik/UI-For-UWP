using Telerik.Charting;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a step area series.
    /// </summary>
    public class StepAreaSeries : AreaSeries
    {
        /// <summary>
        /// Identifies the <see cref="RisersPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RisersPositionProperty =
            DependencyProperty.Register(nameof(RisersPosition), typeof(StepSeriesRisersPosition), typeof(StepAreaSeries), new PropertyMetadata(StepSeriesRisersPosition.Default, OnRisersPositionChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="StepAreaSeries"/> class.
        /// </summary>
        public StepAreaSeries()
        {
            this.DefaultStyleKey = typeof(StepAreaSeries);
        }

        /// <summary>
        /// Gets or sets the <see cref="RisersPosition"/> that will be used to draw the series.
        /// </summary>
        public StepSeriesRisersPosition RisersPosition
        {
            get
            {
                return (StepSeriesRisersPosition)this.GetValue(RisersPositionProperty);
            }
            set
            {
                this.SetValue(RisersPositionProperty, value);
            }
        }

        internal override LineRenderer CreateRenderer()
        {
            return new StepAreaRenderer();
        }

        internal override Charting.CategoricalStrokedSeriesModel CreateModel()
        {
            return new StepSeriesModel();
        }

        private static void OnRisersPositionChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            StepAreaSeries series = target as StepAreaSeries;
            ((StepSeriesModel)series.model).RisersPosition = (StepSeriesRisersPosition)args.NewValue;
        }
    }
}
