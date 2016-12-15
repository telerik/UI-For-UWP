using System;

namespace Telerik.Charting
{
    // TODO: Minor ticks are not implemented at the moment.
    internal class MinorTickModel : AxisTickModel
    {
        internal override TickType Type
        {
            get
            {
                return TickType.Minor;
            }
        }
    }
}
