using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data.HexView
{
    /// <summary>
    /// Layout definition that arranges the hexagonal elements in a honeycomb pattern.
    /// If the <see cref="HexLayoutDefinitionBase.Orientation"/> is vertical, the hexagons are flat,
    /// otherwise they are rotated 90 degrees and become angled. The elements are arranged so that the rows
    /// are completely filled (this leaves some spacing between the elements).
    /// </summary>
    [Bindable]
    public class FlatLooseHexLayoutDefinition : HexLayoutDefinitionBase
    {
        internal override HexLayoutStrategyBase CreateStrategy(HexItemModelGenerator generator, RadHexView view)
        {
            return new FlatLooseHexLayoutStrategy(generator, view, this);
        }
    }
}
