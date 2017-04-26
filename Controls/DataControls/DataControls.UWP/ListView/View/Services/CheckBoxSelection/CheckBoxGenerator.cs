using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class CheckBoxGenerator : ItemGeneratorBase<GeneratedItemModel, ItemInfo>
    {
        public CheckBoxGenerator(IUIContainerGenerator<GeneratedItemModel, ItemInfo> owner)
            : base(owner)
        {
        }

        internal override GeneratedItemModel GenerateContainer(ItemInfo context, object key)
        {
            var container = base.GenerateContainer(context, key);
            container.ItemInfo = context;
            return container;
        }

        internal override object ContainerTypeForItem(ItemInfo context)
        {
            return typeof(ItemCheckBoxControl);
        }

        protected override GeneratedItemModel GenerateContainerForItem(ItemInfo context, object containerType)
        {
            var uiContainer = this.Owner.GenerateContainerForItem(context, containerType);

            return new GeneratedItemModel { Container = uiContainer, ContainerType = containerType, ItemInfo = context };
        }
    }
}
