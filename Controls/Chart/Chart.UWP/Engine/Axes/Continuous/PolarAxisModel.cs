using System;

namespace Telerik.Charting
{
    internal class PolarAxisModel : LinearAxisModel
    {
        internal override int DefaultTickCount
        {
            get
            {
                return 5;
            }
        }

        internal override AxisModelLayoutStrategy CreateLayoutStrategy()
        {
            return new PolarAxisLayoutStrategy();
        }

        internal override void UpdateActualRange(AxisUpdateContext context)
        {
            base.UpdateActualRange(context);

            // actual range always starts from zero
            this.actualRange.minimum = 0;
        }

        internal override double TransformValue(double value)
        {
            // negative values are not defined in polar coordinates
            if (value < 0)
            {
                // take the absolute value
                value *= -1;
            }

            return value;
        }
    }
}
