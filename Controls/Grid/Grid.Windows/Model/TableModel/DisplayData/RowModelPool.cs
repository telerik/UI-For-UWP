using System;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class RowModelPool : NodePool<GridRowModel, RowGenerationContext>
    {
        private RowModelGenerator rowModelGenerator;

        public RowModelPool(ITable table, RowModelGenerator modelGenerator, BaseLayout layout)
            : base(table, modelGenerator, layout, false)
        {
            this.rowModelGenerator = modelGenerator;
        }

        internal override GridRowModel GenerateAndPrepareContainer(ref ItemInfo itemInfo)
        {
            GridRowModel decorator = this.rowModelGenerator.GenerateContainer(new RowGenerationContext(itemInfo, false));
            decorator.ItemInfo = itemInfo;
            this.rowModelGenerator.PrepareContainerForItem(decorator);
            return decorator;
        }

        internal override GridRowModel GenerateAndPrepareFrozenContainer(ref ItemInfo itemInfo)
        {
            GridRowModel decorator = this.rowModelGenerator.GenerateContainer(new RowGenerationContext(itemInfo, true));
            decorator.ItemInfo = itemInfo;
            this.rowModelGenerator.PrepareContainerForItem(decorator);
            return decorator;
        }
    }
}
