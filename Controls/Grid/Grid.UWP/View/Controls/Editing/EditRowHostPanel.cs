using System;
using System.Linq;
using Telerik.Core;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents the panel that hosts the editing UI of the <see cref="RadDataGrid"/> control.
    /// </summary>
    public class EditRowHostPanel : Windows.UI.Xaml.Controls.Grid
    {
        internal IGridView Owner { get; set; }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);

            var grid = this.Owner as RadDataGrid;

            var editorCell = grid.Model.GetCellEditorModel(this);
            var editorLine = grid.Model.GetEditRowLine();

            if (editorCell != null)
            {
                editorCell.DesiredSize = RadSize.Invalid;
            }

            // adjust the height of the row behind the edit row taking into account the grid lines(if any and if there is no group header before the item).
            var gridlineLength = 0d;
            var displayedElement = grid.Model.RowPool.GetDisplayedElement(editorLine - 1);
            bool shouldUpdateRowHeight = editorLine == 0 || displayedElement == null ? false : displayedElement.ContainerType != typeof(DataGridGroupHeader);

            if (editorLine > 0 && grid.HasHorizontalGridLines && shouldUpdateRowHeight)
            {
                gridlineLength += grid.GridLinesThickness;
            }
            grid.Model.UpdateRowPoolRenderInfo(grid.Model.GetEditRowLine(), size.Height + gridlineLength);

            grid.updateService.RegisterUpdate(UpdateFlags.AffectsDecorations);

            return base.MeasureOverride(availableSize);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new EditRowHostPanelAutomationPeer(this);
        }
    }
}