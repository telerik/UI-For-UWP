using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class FixedSegmentLayout : Layout
    {
        public FixedSegmentLayout(double startAngle)
        {
            this.SegmentAngleLength = 45;
            this.StartAngle = startAngle;
        }

        public double SegmentAngleLength { get; private set; }

        public override RadialLayoutSlot GetLayoutSlotAtPosition(RingModelBase model, RadialSegment segment)
        {
            if (segment.TargetItem.Index >= 8)
            {
                return RadialLayoutSlot.Invalid;
            }

            return new RadialLayoutSlot
            {
                InnerRadius = model.InnerRadius,
                OuterRadius = model.OuterRadius,
                StartAngle = (this.StartAngle + this.SegmentAngleLength * segment.TargetItem.Index) % 360,
                SweepAngle = this.SegmentAngleLength
            };
        }
    }
}
