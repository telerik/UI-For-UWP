namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class RateOfChangeIndicatorDataSource : MomentumIndicatorDataSource
    {
        protected override double CalculateValue(double currentValue, double olderValue)
        {
            return (currentValue - olderValue) / olderValue * 100;
        }
    }
}
