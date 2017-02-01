using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal interface IGeneratorsHost
    {
        IUIContainerGenerator<GridCellModel, CellGenerationContext> CellItemGenerator { get; }
        IUIContainerGenerator<GridHeaderCellModel, ColumnGenerationContext> ColumnItemGenerator { get; }
        IUIContainerGenerator<GridRowModel, RowGenerationContext> RowItemGenerator { get; }

        IUIContainerGenerator<GridEditRowModel, RowGenerationContext> EditRowItemGenerator { get; }
        IUIContainerGenerator<GridCellEditorModel, CellGenerationContext> CellEditorItemGenerator { get; }
    }
}
