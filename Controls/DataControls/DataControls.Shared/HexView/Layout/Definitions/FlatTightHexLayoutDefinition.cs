using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data.HexView
{
    /// <summary>
    /// Layout definition that arranges the hexagonal elements in a honeycomb pattern.
    /// If the <see cref="HexLayoutDefinitionBase.Orientation"/> is vertical, the hexagons are flat,
    /// otherwise they are rotated 90 degrees and become angled. The elements are arranged alternatively
    /// on the current and the next row so that there is no spacing between them.
    /// </summary>
    [Bindable]
    public class FlatTightHexLayoutDefinition : HexLayoutDefinitionBase
    {
        internal override HexLayoutStrategyBase CreateStrategy(HexItemModelGenerator generator, RadHexView view)
        {
            return new FlatTightHexLayoutStrategy(generator, view, this);
        }
    }
}
