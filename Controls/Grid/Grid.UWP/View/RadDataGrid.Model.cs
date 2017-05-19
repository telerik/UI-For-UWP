using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Telerik.Core;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Model;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Grid.View;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid
    {
        private GridModel model;

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        double IView.ViewportWidth
        {
            get
            {
                return this.lastAvailableSize.Width;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        double IView.ViewportHeight
        {
            get
            {
                return this.lastAvailableSize.Height - this.ColumnHeadersHost.ActualHeight;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        bool IElementPresenter.IsVisible
        {
            get
            {
                return this.Visibility == Visibility.Visible;
            }
        }

        internal GridModel Model
        {
            get
            {
                return this.model;
            }
        }

        void IGridView.SetScrollPosition(RadPoint point, bool updateUI, bool updateScrollViewer)
        {
            this.SetHorizontalOffset(point.X, updateUI, updateScrollViewer);
            this.SetVerticalOffset(point.Y, updateUI, updateScrollViewer);
        }

        void IGridView.ProcessDataChangeFlags(DataChangeFlags flags)
        {
            // TODO: Decide whether we need to keep the edit row synchronized with the DataView
            this.CancelEdit();
        }

        void IGridView.OnDataStatusChanged(DataProviderStatus status)
        {
            this.visualStateService.UpdateDataLoadingStatus(status);
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void IElementPresenter.RefreshNode(object node)
        {
        }

        void IGridView.Arrange(Node node)
        {
            GridEditRowModel editRow = node as GridEditRowModel;

            if (editRow != null)
            {
                this.ArrangeEditRow(editRow);
                return;
            }

            GridRowModel row = node as GridRowModel;
            if (row != null)
            {
                RadDataGrid.ArrangeRow(row);
                return;
            }

            GridHeaderCellModel headerCell = node as GridHeaderCellModel;
            if (headerCell != null)
            {
                this.ArrangeHeaderCell(headerCell);
                return;
            }

            GridCellEditorModel editCell = node as GridCellEditorModel;
            if (editCell != null)
            {
                this.ArrangeEditorCell(editCell);
            }

            GridCellModel cell = node as GridCellModel;
            if (cell != null)
            {
                this.ArrangeCell(cell);
            }
        }

        void IGridView.RebuildUI()
        {
            // In case the column size is not fixed, we should ensure that the column headers are measured before we measure the cells.
            this.cellsPanel.isDirectMeasure = true;

            if (this.visualStateLayerCache != null)
            {
                this.visualStateLayerCache.UpdateHoverDecoration(RadRect.Empty);
            }

            if (this.frozenVisualStateLayerCache != null)
            {
                this.frozenVisualStateLayerCache.UpdateHoverDecoration(RadRect.Empty);
            }

            this.InvalidatePanelsMeasure();
        }

        void IGridView.InvalidateHeadersPanelMeasure()
        {
            this.InvalidateHeadersMeasure();
        }

        void IGridView.InvalidateHeadersPanelArrange()
        {
            this.InvalidateHeadersArrange();
        }

        void IGridView.InvalidateCellsPanelMeasure()
        {
            this.InvalidateCellsMeasure();
        }

        void IGridView.InvalidateCellsPanelArrange()
        {
            this.InvalidateCellsArrange();
        }

        void IGridView.ApplyLayersClipping(RadRect clip, RadRect frozenRect)
        {
            var cancelButtonHeight = 40;

            RadRect editorLayout = RadRect.Empty;

            if (this.editService.IsEditing && this.UserEditMode == DataGridUserEditMode.Inline && this.EditRowLayer.EditorLayoutSlots.Any())
            {
                editorLayout = this.EditRowLayer.EditorLayoutSlots.First().Value;
            }

            var editButtonExtendHeight = editorLayout != RadRect.Empty && editorLayout.Y != 0 ? cancelButtonHeight : 0;

            var verticalGridLineOffset = this.HasVerticalGridLines ? this.GridLinesThickness : 0.0;
            var horizontalGridLineOffset = this.HasHorizontalGridLines ? this.GridLinesThickness : 0.0;

            if (this.FrozenColumnCount > 0)
            {
                var trimmedClip = new Windows.Foundation.Rect(clip.X + verticalGridLineOffset, 0, clip.Width, this.columnHeadersPanel.ActualHeight);
                this.columnHeadersPanel.Clip = new Windows.UI.Xaml.Media.RectangleGeometry { Rect = trimmedClip };
            }
            else
            {
                this.columnHeadersPanel.Clip = null;
            }

            var adjustedFrozenClip = new RadRect(frozenRect.X - verticalGridLineOffset, frozenRect.Y, frozenRect.Width + verticalGridLineOffset, frozenRect.Height);

            var adjustedClip = new RadRect(clip.X - verticalGridLineOffset, clip.Y, clip.Width, clip.Height + editButtonExtendHeight);

            foreach (var item in this.ContentLayers)
            {
                item.ApplyClip(adjustedClip);
            }

            var decorationLayerClip = new RadRect(adjustedClip.X, adjustedClip.Y, adjustedClip.Width, adjustedClip.Height + horizontalGridLineOffset);
            this.DecorationLayer.ApplyClip(decorationLayerClip);

            if (this.FrozenColumnCount > 0)
            {
                var frozendecorationClip = new RadRect(adjustedFrozenClip.X, adjustedFrozenClip.Y, adjustedFrozenClip.Width, adjustedFrozenClip.Height + horizontalGridLineOffset);

                this.FrozenColumnsContentLayer.ApplyClip(adjustedFrozenClip);
                this.frozenDecorationLayer.ApplyClip(frozendecorationClip);
                this.GroupHeadersContentLayer.ApplyClip(new RadRect(0, adjustedFrozenClip.Y, this.CellsPanel.ActualWidth, this.CellsPanel.ActualHeight), false);
            }
            else
            {
                var content = this.FrozenColumnsContentLayer.VisualElement;
                if (content != null)
                {
                    content.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, 0, 0) };
                }
                var decoration = this.frozenDecorationLayer.VisualElement;
                if (decoration != null)
                {
                    decoration.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, 0, 0) };
                }
            }

            if (this.FrozenEditRowLayer.EditorLayoutSlots.Count > 0)
            {
                var CancelButtonHeight = this.frozenEditRowLayer.EditorLayoutSlots.Values.Select(x => x.Height).FirstOrDefault();
                var frozenEditRowClip = new RadRect(adjustedFrozenClip.X, adjustedFrozenClip.Y, adjustedFrozenClip.Width, adjustedFrozenClip.Height + CancelButtonHeight);

                this.FrozenEditRowLayer.ApplyClip(frozenEditRowClip);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        RadSize IElementPresenter.MeasureContent(object owner, object content)
        {
            GridRowModel row = owner as GridRowModel;
            if (row != null)
            {
                return this.MeasureRow(row);
            }

            GridHeaderCellModel headerCell = owner as GridHeaderCellModel;
            if (headerCell != null)
            {
                return this.MeasureHeaderCell(headerCell);
            }

            GridCellEditorModel editCell = owner as GridCellEditorModel;
            if (editCell != null)
            {
                return this.MeasureEditCell(editCell);
            }

            GridCellModel cell = owner as GridCellModel;
            if (cell != null)
            {
                return this.MeasureCell(cell);
            }

            return RadSize.Empty;
        }

        internal void UpdateHorizontalPosition()
        {
            this.SetHorizontalOffset(this.ScrollViewer.HorizontalOffset, false, false);
        }

        internal RadRect InflateCellHorizontally(GridCellModel cell, RadRect rect)
        {
            if (cell.Column.ItemInfo.Id > 0)
            {
                var thickness = this.HasVerticalGridLines ? this.gridLinesThicknessCache : 0;
                rect.X += thickness;
                rect.Width -= thickness;
            }

            if (rect.Width < 0)
            {
                rect.Width = 0;
            }

            return rect;
        }

        internal RadRect InflateEditCellHorizontally(GridCellEditorModel cell, RadRect rect)
        {
            if (cell.Column.ItemInfo.Id > 0)
            {
                var thickness = this.HasVerticalGridLines ? this.gridLinesThicknessCache : 0;
                rect.X += thickness;
                rect.Width -= thickness;
            }

            return rect;
        }

        internal RadRect InflateCellVertically(GridCellModel cell, RadRect rect)
        {
            var row = cell.ParentRow;

            if (row != null && this.decorationLayerCache != null)
            {
                var thickness = this.HasHorizontalGridLines ? this.gridLinesThicknessCache : 0;
                if (this.decorationLayerCache.HasHorizontalLine(row.ItemInfo))
                {
                    rect.Y += thickness;
                    rect.Height -= thickness;
                }
            }

            if (rect.Height < 0)
            {
                rect.Height = 0;
            }

            return rect;
        }

        internal RadRect InflateEditCellVertically(GridCellEditorModel cell, RadRect rect)
        {
            var row = cell.ParentRow as GridEditRowModel;

            if (row != null && this.decorationLayerCache != null)
            {
                var thickness = this.HasHorizontalGridLines ? this.gridLinesThicknessCache : 0;

                if (this.decorationLayerCache.HasHorizontalLine(row.ReadOnlyRowInfo))
                {
                    rect.Y += thickness;
                }
            }

            return rect;
        }

        internal RadRect InflateEditRowVertically(GridEditRowModel row, RadRect rect)
        {
            // the editrow uses the size of the row behind it(which has already been precalculated in the EditRowHostPanel and takes into acount the gridlines(if any).
            if (row != null && this.decorationLayerCache != null)
            {
                var line = row.ReadOnlyRowInfo.LayoutInfo.Line;
                var visibleLines = this.Model.RowPool.Layout.VisibleLineCount;
                var height = 0d;

                var element = this.Model.RowPool.GetDisplayedElement(line + 1);

                if (element != null && line != visibleLines - 1 && this.HasHorizontalGridLines && element.ContainerType != typeof(DataGridGroupHeader))
                {
                    height += this.GridLinesThickness;
                }

                var layoutSlot = this.Model.RowPool.GetDisplayedElement(line).LayoutSlot;
                var arrangeRect = new RadRect(rect.X, rect.Y, rect.Width, layoutSlot.Height + height);

                return arrangeRect;
            }

            return rect;
        }

        private static Size ArrangeRow(GridRowModel row)
        {
            var arrangeRect = row.layoutSlot;

            // TODO: Pass this to the content layer(s) in case needed.
            var container = row.Container as UIElement;

            if (container != null)
            {
                container.Arrange(arrangeRect.ToRect());
            }

            var tuple = row.Container as Tuple<UIElement, UIElement>;

            if (tuple != null)
            {
                if (tuple.Item1 != null)
                {
                    tuple.Item1.Arrange(arrangeRect.ToRect());
                }

                if (tuple.Item2 != null)
                {
                    tuple.Item2.Arrange(arrangeRect.ToRect());
                }
            }

            return new Size(arrangeRect.Width, arrangeRect.Height);
        }

        private void ArrangeEditRow(GridEditRowModel row)
        {
            this.EditRowLayer.ArrangeEditRow(row);
        }

        private RadSize MeasureRow(GridRowModel row)
        {
            // TODO: Consider different Layer model so that RowGroups can be measured also.
            UIElement container = row.Container as UIElement;
            if (container != null)
            {
                container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                row.DesiredSize = GridModel.DoubleArithmetics.Ceiling(container.DesiredSize.ToRadSize());
            }

            var tuple = row.Container as Tuple<UIElement, UIElement>;
            if (tuple != null)
            {
                if (tuple.Item1 != null)
                {
                    tuple.Item1.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    row.DesiredSize = GridModel.DoubleArithmetics.Ceiling(tuple.Item1.DesiredSize.ToRadSize());
                }

                if (tuple.Item2 != null)
                {
                    tuple.Item2.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    row.DesiredSize = GridModel.DoubleArithmetics.Ceiling(tuple.Item2.DesiredSize.ToRadSize());
                }
            }

            return new RadSize(this.lastAvailableSize.Width, row.DesiredSize.Height);
        }

        private RadSize MeasureEditCell(GridCellEditorModel cell)
        {
            var layer = this.EditRowLayer;

            // TODO: consider if content needs to be measured with constraint in stretch mode.
            var availableWidth = cell.Column.SizeMode == DataGridColumnSizeMode.Fixed ? cell.Column.Width : double.PositiveInfinity;

            RadSize size = GridModel.DoubleArithmetics.Ceiling(layer.MeasureCell(cell, availableWidth).ToRadSize());

            cell.Column.AutoWidth = Math.Max(cell.Column.AutoWidth, size.Width);

            cell.DesiredSize = new RadSize(cell.Column.AutoWidth, size.Height);

            return cell.DesiredSize;
        }

        private RadSize MeasureCell(GridCellModel cell)
        {
            ContentLayer layer = this.GetContentLayerForColumn(cell.Column);

            // TODO: consider if content needs to be measured with constraint in stretch mode.
            var availableWidth = cell.Column.SizeMode == DataGridColumnSizeMode.Fixed ? cell.Column.Width : double.PositiveInfinity;

            RadSize size = GridModel.DoubleArithmetics.Ceiling(layer.MeasureCell(cell, availableWidth).ToRadSize());

            if (this.HasHorizontalGridLines && !this.decorationLayerCache.IsFirstLine(cell.ParentRow.ItemInfo))
            {
                size.Height += this.GridLinesThickness;
            }
            if (this.HasVerticalGridLines)
            {
                size.Width += this.GridLinesThickness;
            }

            cell.DesiredSize = size;
            cell.Column.AutoWidth = Math.Max(cell.Column.AutoWidth, size.Width);

            return cell.DesiredSize;
        }

        private RadSize MeasureHeaderCell(GridHeaderCellModel cell)
        {
            // TODO: consider if content needs to be measured with constraint in stretch mode.
            var availableWidth = cell.Column.SizeMode == DataGridColumnSizeMode.Fixed ? cell.Column.Width : double.PositiveInfinity;
            var size = GridModel.DoubleArithmetics.Ceiling(cell.Column.MeasureCell(cell, availableWidth));
            if (this.HasVerticalGridLines)
            {
                size.Width -= this.GridLinesThickness;
            }

            cell.DesiredSize = size;
            cell.Column.AutoWidth = Math.Max(cell.Column.AutoWidth, size.Width);

            return cell.DesiredSize;
        }

        private void ArrangeEditorCell(GridCellEditorModel cell)
        {
            this.EditRowLayer.ArrangeCell(cell);
        }

        private void ArrangeCell(GridCellModel cell)
        {
            if (this.decorationLayerCache != null)
            {
                this.decorationLayerCache.ArrangeCellDecoration(cell);
            }

            var layer = this.GetContentLayerForColumn(cell.Column);
            layer.ArrangeCell(cell);
        }

        private void ArrangeHeaderCell(GridHeaderCellModel cell)
        {
            var container = cell.Container as DataGridColumnHeader;
            if (container == null)
            {
                return;
            }

            cell.Column.LayoutWidth = cell.layoutSlot.Width;
            cell.Column.ActualWidth = cell.layoutSlot.Width;
            if (this.HasVerticalGridLines)
            {
                cell.Column.ActualWidth -= this.gridLinesThicknessCache;
            }

            var rect = this.InflateCellHorizontally(cell, cell.layoutSlot);

            Canvas.SetLeft(container, rect.X);
            Canvas.SetTop(container, rect.Y);

            // Overcome the differences in the DesiredSize of the header and the ArrangeSize, coming from the NodePool.
            container.AllowArrange = true;

            container.ArrangeRestriction = new Size(rect.Width, rect.Height);

            // The InvalidateArrange call is needed as the arrange size may be same and an actual Arrange pass will not be triggered
            container.InvalidateArrange();
            container.Arrange(rect.ToRect());

            container.AllowArrange = false;
        }
    }
}