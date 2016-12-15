using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data.HexView
{
    /// <summary>
    /// Layout definition that arranges the hexagonal elements in a honeycomb pattern.
    /// If the <see cref="HexLayoutDefinitionBase.Orientation"/> is vertical, the hexagons are angled,
    /// otherwise they are rotated 90 degrees and become flat.
    /// </summary>
    [Bindable]
    public class AngledHexLayoutDefinition : HexLayoutDefinitionBase
    {
        internal override HexLayoutStrategyBase CreateStrategy(HexItemModelGenerator generator, RadHexView view)
        {
            return new AngledHexLayoutStrategy(generator, view, this);
        }
    }
}
