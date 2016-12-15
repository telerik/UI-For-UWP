using Telerik.Core;

namespace Telerik.Charting
{
    internal class AxisLabelModel : ContentNode
    {
        internal decimal normalizedPosition;
        internal RadPoint transformOffset; // the visual layer may apply some transformations over the node
        internal RadSize untransformedDesiredSize; // the size before any visual transformations are applied
    }
}
