using System;

namespace Telerik.Charting
{
    internal class MajorTickModel : AxisTickModel
    {
        internal override TickType Type
        {
            get
            {
                return TickType.Major;
            }
        }
    }
}
