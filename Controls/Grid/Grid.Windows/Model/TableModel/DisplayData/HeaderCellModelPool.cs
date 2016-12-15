using System;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class ColumnHeaderPool : NodePool<GridHeaderCellModel, ColumnGenerationContext>
    {
        private ColumnModelGenerator columnModelGenerator;

        public ColumnHeaderPool(ITable table, ColumnModelGenerator modelGenerator, BaseLayout layout)
            : base(table, modelGenerator, layout, true)
        {
            this.columnModelGenerator = modelGenerator;
            this.IsBufferNeeded = false;
        }

        internal override GridHeaderCellModel GenerateAndPrepareContainer(ref ItemInfo itemInfo)
        {
            var column = itemInfo.Item as DataGridColumn;

            GridHeaderCellModel decorator = this.columnModelGenerator.GenerateContainer(new ColumnGenerationContext(itemInfo, column.IsFrozen));
            decorator.ItemInfo = itemInfo;

            this.columnModelGenerator.PrepareContainerForItem(decorator);
            return decorator;
        }

        internal override GridHeaderCellModel GenerateAndPrepareFrozenContainer(ref ItemInfo itemInfo)
        {
            throw new NotImplementedException();
        }
    }
}