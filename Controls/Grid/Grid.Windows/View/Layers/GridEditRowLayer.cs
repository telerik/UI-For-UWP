using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal class GridEditRowLayer : SharedUILayer
    {
        private bool focusFirstEditor;

        public GridEditRowLayer()
        {
            this.EditorLayoutSlots = new Dictionary<object, RadRect>();
        }

        public Dictionary<object, RadRect> EditorLayoutSlots
        {
            get;
            private set;
        }

        public void ScheduleFirstEditorForFocus()
        {
            this.focusFirstEditor = true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        internal Size MeasureCell(GridCellEditorModel cell, double availableWidth)
        {
            return cell.Column.MeasureEditCell(cell, availableWidth).ToSize();
        }

        internal void ArrangeCell(GridCellEditorModel cell)
        {
            var pair = cell.Container as Tuple<FrameworkElement, FrameworkElement, FrameworkElement>;
            if (pair == null || pair.Item2 == null)
            {
                return;
            }

            RadRect layoutSlot = cell.Column.ApplyLayoutSlotAlignment(cell);

            var rect = this.Owner.InflateEditCellHorizontally(cell, layoutSlot);
            rect = this.Owner.InflateEditCellVertically(cell, rect);

            if (this.Owner.EditRowLayer.EditorLayoutSlots.ContainsKey(cell))
            {
                this.Owner.EditRowLayer.EditorLayoutSlots[cell] = rect;
            }
            else
            {
                this.Owner.EditRowLayer.EditorLayoutSlots.Add(cell, rect);
            }

            pair.Item2.Arrange(rect.ToRect());

            if (this.focusFirstEditor)
            {
                var control = pair.Item1 as Control;
                if (control != null && control.IsTabStop)
                {
                    control.Focus(FocusState.Programmatic);
                }

                this.focusFirstEditor = false;
            }
        }

        internal void ArrangeEditRow(GridEditRowModel row)
        {
            // TODO: Pass this to the content layer(s) in case needed.
            var editRow = row.Container as DataGridEditRow;

            if (editRow != null)
            {
                var arrangeRect = row.LayoutSlot;
                var rect = this.Owner.InflateEditRowVertically(row, arrangeRect);

                editRow.Arrange(rect.ToRect());

                var dispayButtonOnTop = this.CanDisplayCancelButtonOnTop(row);

                editRow.PositionCloseButton(rect, dispayButtonOnTop, row.IsFrozen);
            }
        }

        protected internal override void AddVisualChild(UIElement child)
        {
            base.AddVisualChild(child);

            Canvas.SetZIndex(child, ZIndexConstants.EditRowContentZIndex);
        }

        private bool CanDisplayCancelButtonOnTop(GridEditRowModel model)
        {
            double offset = this.Owner.Model.RowPool.FrozenContainers.Any() ? this.Owner.Model.RowPool.FrozenContainers.Max(c => c.LayoutSlot.Bottom) : 0;
            var row = model.Container as DataGridEditRow;

            return model.ReadOnlyRowInfo.Slot != 0 && offset <= model.LayoutSlot.Y - row.CancelButton.ActualHeight;
        }
    }
}
