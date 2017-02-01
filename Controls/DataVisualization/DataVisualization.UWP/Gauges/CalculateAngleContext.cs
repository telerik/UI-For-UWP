using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// Contains data to calculate an angle based on a value.
    /// </summary>
    internal class CalculateAngleContext
    {
        internal double startValue;
        internal double value;

        internal double minAngle;
        internal double maxAngle;

        internal double gaugeLogicalLength;
        internal double gaugePhysicalLength;

        public CalculateAngleContext(double startValue, double value, double minAngle, double maxAngle, double gaugeLogicalLength, double gaugePhysicalLength)
        {
            this.startValue = startValue;
            this.value = value;

            this.minAngle = minAngle;
            this.maxAngle = maxAngle;

            this.gaugeLogicalLength = gaugeLogicalLength;
            this.gaugePhysicalLength = gaugePhysicalLength;
        }
    }
}
