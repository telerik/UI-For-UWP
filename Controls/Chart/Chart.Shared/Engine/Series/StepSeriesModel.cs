using Telerik.Core;
using Telerik.UI.Xaml.Controls.Chart;

namespace Telerik.Charting
{
    internal class StepSeriesModel : CategoricalStrokedSeriesModel
    {
        internal static readonly int RisersPositionPropertyKey = PropertyKeys.Register(typeof(StepSeriesModel), "RisersPosition", ChartAreaInvalidateFlags.InvalidateSeries);

        internal override AxisPlotMode DefaultPlotMode
        {
            get
            {
                return AxisPlotMode.BetweenTicks;
            }
        }

         internal StepSeriesRisersPosition RisersPosition
        {
            get
            {
                return this.GetTypedValue<StepSeriesRisersPosition>(RisersPositionPropertyKey, StepSeriesRisersPosition.Default);
            }
            set
            {
                this.SetValue(RisersPositionPropertyKey, value);
            }
        }
    }
}
