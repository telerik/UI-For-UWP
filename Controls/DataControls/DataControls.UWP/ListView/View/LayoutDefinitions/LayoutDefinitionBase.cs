using Telerik.Core;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView.Model;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    /// <summary>
    /// A base class for the <see cref="RadListView"/> layout definitions.
    /// </summary>
    public abstract class LayoutDefinitionBase : ViewModelBase
    {
        internal abstract BaseLayoutStrategy CreateStrategy(ItemModelGenerator generator, IOrientedParentView view);

        internal abstract void UpdateStrategy(BaseLayoutStrategy strategy);
    }
}
