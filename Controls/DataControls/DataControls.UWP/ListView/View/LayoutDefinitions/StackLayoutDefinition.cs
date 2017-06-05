using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView.Model;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// Represents a UI virtualization strategy definition for a stack layout strategy.
    /// </summary>
    public class StackLayoutDefinition : LayoutDefinitionBase
    {
        internal override BaseLayoutStrategy CreateStrategy(ItemModelGenerator generator, IOrientedParentView view)
        {
            return new StackLayoutStrategy(generator, view) { IsHorizontal = view.Orientation == Orientation.Horizontal };
        }

        internal override void UpdateStrategy(BaseLayoutStrategy strategy)
        {
        }
    }
}
