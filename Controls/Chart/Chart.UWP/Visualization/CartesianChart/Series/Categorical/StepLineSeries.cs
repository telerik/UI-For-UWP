using Telerik.Charting;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a step line series.
    /// </summary>
    public class StepLineSeries : LineSeries
    {
        /// <summary>
        /// Identifies the <see cref="RisersPosition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RisersPositionProperty =
            DependencyProperty.Register(nameof(RisersPosition), typeof(StepSeriesRisersPosition), typeof(StepLineSeries), new PropertyMetadata(StepSeriesRisersPosition.Default, OnRisersPositionChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="StepLineSeries"/> class.
        /// </summary>
        public StepLineSeries()
        {
            this.DefaultStyleKey = typeof(StepLineSeries);
        }

        /// <summary>
        /// Gets or sets the <see cref="RisersPosition"/> that will be used to draw the series.
        /// </summary>
        public StepSeriesRisersPosition RisersPosition
        {
            get
            {
                return (StepSeriesRisersPosition)GetValue(RisersPositionProperty);
            }
            set
            {
                this.SetValue(RisersPositionProperty, value);
            }
        }

        internal override CategoricalStrokedSeriesModel CreateModel()
        {
            return new StepSeriesModel();
        }

        internal override LineRenderer CreateRenderer()
        {
            return new StepLineRenderer();
        }

        private static void OnRisersPositionChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            StepLineSeries series = target as StepLineSeries;
            ((StepSeriesModel)series.model).RisersPosition = (StepSeriesRisersPosition)args.NewValue;
        }      
    }
}
