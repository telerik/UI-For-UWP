using System;
using System.Windows;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class CellModelGenerator : ItemModelGenerator<GridCellModel, CellGenerationContext>
    {
        public CellModelGenerator(IUIContainerGenerator<GridCellModel, CellGenerationContext> owner)
            : base(owner)
        {
        }

        internal override void PrepareContainerForItem(GridCellModel decorator)
        {
            this.Owner.PrepareContainerForItem(decorator);
            this.Owner.SetOpacity(decorator, 1);
        }

        internal override object ContainerTypeForItem(CellGenerationContext context)
        {
            return this.Owner.GetContainerTypeForItem(context);
        }

        protected override void ClearContainerForItem(GridCellModel decorator)
        {
            this.Owner.ClearContainerForItem(decorator);
        }

        protected override GridCellModel GenerateContainerForItem(CellGenerationContext context, object containerType)
        {
            object element = this.Owner.GenerateContainerForItem(context, containerType);
            return new GridCellModel() { Container = element, ContainerType = containerType };
        }
    }
}