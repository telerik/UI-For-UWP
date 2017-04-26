using Telerik.UI.Xaml.Controls.Data.ListView.Model;

namespace Telerik.UI.Xaml.Controls.Data.ContainerGeneration
{
    internal class ItemModelGenerator : ItemGeneratorBase<GeneratedItemModel, ItemGenerationContext>
    {
        public ItemModelGenerator(IUIContainerGenerator<GeneratedItemModel, ItemGenerationContext> owner)
            : base(owner)
        {
        }

        protected override GeneratedItemModel GenerateContainerForItem(ItemGenerationContext context, object containerType)
        {
            var uiContainer = this.Owner.GenerateContainerForItem(context, containerType);

            return new GeneratedItemModel { Container = uiContainer, ContainerType = containerType };
        }
    }
}
