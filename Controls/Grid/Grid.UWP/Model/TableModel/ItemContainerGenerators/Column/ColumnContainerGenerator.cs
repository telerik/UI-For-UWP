using System.Windows;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Grid;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class ColumnModelGenerator : ItemModelGenerator<GridHeaderCellModel, ColumnGenerationContext>
    {
        public ColumnModelGenerator(IUIContainerGenerator<GridHeaderCellModel, ColumnGenerationContext> owner)
            : base(owner)
        {
        }

        internal override void PrepareContainerForItem(GridHeaderCellModel decorator)
        {
            this.Owner.PrepareContainerForItem(decorator);
        }

        internal override object ContainerTypeForItem(ColumnGenerationContext item)
        {
            return this.Owner.GetContainerTypeForItem(item);
        }

        protected override void ClearContainerForItem(GridHeaderCellModel decorator)
        {
            this.Owner.ClearContainerForItem(decorator);
        }

        protected override GridHeaderCellModel GenerateContainerForItem(ColumnGenerationContext context, object containerType)
        {
            var element = this.Owner.GenerateContainerForItem(context, containerType);
            return new GridHeaderCellModel() { Container = element, ContainerType = containerType, Column = context.Info.Item as DataGridColumn, IsFrozen = context.IsFrozen };
        }
    }
}