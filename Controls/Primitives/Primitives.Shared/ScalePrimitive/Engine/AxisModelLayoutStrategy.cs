using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Primitives.Scale
{
    internal abstract class AxisModelLayoutStrategy
    {
        internal abstract void ArrangeTicks();
        internal abstract RadLine ArrangeLine(RadRect rect);
        internal abstract void ArrangeLabels(RadRect rect);
        internal abstract RadSize UpdateDesiredSize(RadSize availableSize);
    }
}
