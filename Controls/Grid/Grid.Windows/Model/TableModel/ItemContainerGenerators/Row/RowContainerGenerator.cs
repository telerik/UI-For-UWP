using System;
using System.Windows;
using Telerik.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Grid;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class RowModelGenerator : ItemModelGenerator<GridRowModel, RowGenerationContext>
    {
        public RowModelGenerator(IUIContainerGenerator<GridRowModel, RowGenerationContext> owner)
            : base(owner)
        {
        }

        internal override void PrepareContainerForItem(GridRowModel decorator)
        {
            this.Owner.PrepareContainerForItem(decorator);
        }

        internal override object ContainerTypeForItem(RowGenerationContext context)
        {
            if (context == null)
            {
                return null;
            }

            return this.Owner.GetContainerTypeForItem(context);
        }

        protected override void ClearContainerForItem(GridRowModel decorator)
        {
            this.Owner.ClearContainerForItem(decorator);
        }

        protected override GridRowModel GenerateContainerForItem(RowGenerationContext context, object containerType)
        {
            var element = this.Owner.GenerateContainerForItem(context, containerType);
            return new GridRowModel() { Container = element, ContainerType = containerType, IsFrozen = context.IsFrozen };
        }
    }
}