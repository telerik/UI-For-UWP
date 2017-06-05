namespace Telerik.UI.Xaml.Controls.Data.ContainerGeneration
{
    internal class HexItemModelGenerator : ItemGeneratorBase<GeneratedItemModel, object>
    {
        // TODO refactor the ItemModelGenerator related classes
        public HexItemModelGenerator(IUIContainerGenerator<GeneratedItemModel, object> owner)
            : base(owner)
        {
        }

        protected override GeneratedItemModel GenerateContainerForItem(object context, object containerType)
        {
            var uiContainer = this.Owner.GenerateContainerForItem(context, containerType);

            return new GeneratedItemModel { Container = uiContainer, ContainerType = containerType };
        }
    }
}
