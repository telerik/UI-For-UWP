namespace Telerik.Charting
{
    internal class PointSeriesModel : CategoricalStrokedSeriesModel
    {
        internal override AxisPlotMode DefaultPlotMode
        {
            get
            {
                return AxisPlotMode.OnTicksPadded;
            }
        }
    }
}
