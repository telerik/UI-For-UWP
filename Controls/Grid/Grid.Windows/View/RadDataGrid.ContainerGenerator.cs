namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid : IGeneratorsHost
    {
        private IUIContainerGenerator<GridCellModel, CellGenerationContext> cellItemGenerator;
        private IUIContainerGenerator<GridHeaderCellModel, ColumnGenerationContext> columnItemGenerator;
        private IUIContainerGenerator<GridRowModel, RowGenerationContext> rowItemGenerator;
        private IUIContainerGenerator<GridEditRowModel, RowGenerationContext> editRowItemGenerator;
        private IUIContainerGenerator<GridCellEditorModel, CellGenerationContext> cellEditorItemGenerator;

        IUIContainerGenerator<GridCellModel, CellGenerationContext> IGeneratorsHost.CellItemGenerator
        {
            get { return this.cellItemGenerator; }
        }

        IUIContainerGenerator<GridHeaderCellModel, ColumnGenerationContext> IGeneratorsHost.ColumnItemGenerator
        {
            get { return this.columnItemGenerator; }
        }

        IUIContainerGenerator<GridRowModel, RowGenerationContext> IGeneratorsHost.RowItemGenerator
        {
            get { return this.rowItemGenerator; }
        }

        IUIContainerGenerator<GridEditRowModel, RowGenerationContext> IGeneratorsHost.EditRowItemGenerator
        {
            get { return this.editRowItemGenerator; }
        }

        IUIContainerGenerator<GridCellEditorModel, CellGenerationContext> IGeneratorsHost.CellEditorItemGenerator
        {
            get { return this.cellEditorItemGenerator; }
        }
    }
}