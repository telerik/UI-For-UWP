using System;
using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class LinearAxisModel : NumericalAxisModel
    {
        internal override IEnumerable<AxisTickModel> GenerateTicks(ValueRange<decimal> currentVisibleRange)
        {
            // use the decimal type for higher accuracy; see the XML comments on the GetVisibleRange method
            decimal delta = (decimal)this.actualRange.maximum - (decimal)this.actualRange.minimum;
            if (delta <= 0)
            {
                yield break;
            }

            double scale = this.layoutStrategy.GetZoom();
            double step = this.majorStep;
            if (scale != 1d)
            {
                step = NumericalAxisModel.NormalizeStep(step / scale);
            }
            
            decimal tickStep = (decimal)step;
            decimal normalizedTickStep = tickStep / delta;

            currentVisibleRange.minimum -= currentVisibleRange.minimum % normalizedTickStep;
            currentVisibleRange.maximum += normalizedTickStep - (currentVisibleRange.maximum % normalizedTickStep);

            decimal startTick, endTick;
            if (this.IsInverse)
            {
                startTick = Math.Max(0, 1 - currentVisibleRange.maximum);
                endTick = Math.Min(1, 1 - currentVisibleRange.minimum);
            }
            else
            {
                startTick = Math.Max(0, currentVisibleRange.minimum);
                endTick = Math.Min(1, currentVisibleRange.maximum);
            }
        
            decimal currentTick = startTick;
            decimal value = (decimal)this.actualRange.minimum + currentTick * delta;

            int virtualIndex = (int)((value - (decimal)this.actualRange.minimum) / tickStep);

            while (currentTick < endTick || RadMath.AreClose((double)currentTick, (double)endTick))
            {
                AxisTickModel tick = new MajorTickModel();
                tick.normalizedValue = this.IsInverse ? 1 - currentTick : currentTick;
                tick.value = (decimal)this.ReverseTransformValue((double)value);
                tick.virtualIndex = virtualIndex;

                currentTick += normalizedTickStep;
                value += tickStep;
                virtualIndex++;

                yield return tick;
            }
        }
    }
}