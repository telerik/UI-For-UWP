using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class CellEditorsController : CellsController<GridCellEditorModel>
    {
        public CellEditorsController(ITable tableControl, ItemModelGenerator<GridCellEditorModel, CellGenerationContext> cellGenerator)
            : base(tableControl, cellGenerator)
        {
        }

        internal override double GetSlotHeight(int cellSlot)
        {
            if (cellSlot == 0)
            {
                return this.RowPool.RenderInfo.ValueForIndex(cellSlot, true);
            }

            return base.GetSlotHeight(cellSlot);
        }
    }
}
