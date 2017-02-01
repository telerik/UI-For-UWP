using System;
using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class NumericRadialAxisModel : NumericalAxisModel, IRadialAxis
    {
        internal RadialAxisLayoutStrategy radialLayoutStrategy;

        public bool IsLargeArc
        {
            get
            {
                return this.majorStep > 180;
            }
        }

        internal override IEnumerable<AxisTickModel> GenerateTicks(ValueRange<decimal> currentVisibleRange)
        {
            if (this.majorStep <= 0 || this.actualRange.maximum == this.actualRange.minimum)
            {
                yield break;
            }

            decimal tickStep = (decimal)this.majorStep;
            decimal normalizedTickStep = tickStep / 360;

            decimal startTick = 0;
            decimal endTick = 1;
            decimal currentTick = startTick;
            decimal value = 0;

            while (currentTick < endTick || RadMath.AreClose((double)currentTick, (double)endTick))
            {
                AxisTickModel tick = new MajorTickModel();
                tick.normalizedValue = this.IsInverse ? 1 - currentTick : currentTick;
                tick.value = value;
                currentTick += normalizedTickStep;
                value += tickStep;

                yield return tick;
            }
        }

        internal override object ConvertPhysicalUnitsToData(double coordinate, RadRect axisVirtualSize)
        {
            return this.IsInverse ? (360 - coordinate) % 360 : coordinate % 360;
        }

        internal override void UpdateActualRange(AxisUpdateContext context)
        {
            this.actualRange = new ValueRange<double>(0, 360);

            object userStep = this.GetValue(MajorStepPropertyKey);
            if (userStep != null)
            {
                this.majorStep = (double)userStep;
            }
            else
            {
                this.majorStep = 30;
            }
        }

        internal override AxisModelLayoutStrategy CreateLayoutStrategy()
        {
            this.radialLayoutStrategy = new RadialAxisLayoutStrategy();
            return this.radialLayoutStrategy;
        }

        internal override double TransformValue(double value)
        {
            double angle = this.GetChartArea<PolarChartAreaModel>().NormalizeAngle(value);
            return this.IsInverse ? (360 - angle) % 360 : angle;
        }

        protected override bool BuildTicksAndLabels(RadSize availableSize)
        {
            bool result = base.BuildTicksAndLabels(availableSize);

            // last tick and label should not be visible
            AxisTickModel lastTick = this.LastTick;
            if (lastTick != null)
            {
                lastTick.isVisible = false;
                if (lastTick.associatedLabel != null)
                {
                    lastTick.associatedLabel.isVisible = false;
                }
            }

            return result;
        }
    }
}