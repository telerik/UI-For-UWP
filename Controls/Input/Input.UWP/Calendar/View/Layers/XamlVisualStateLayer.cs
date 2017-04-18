using System.Collections.Generic;
using Telerik.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal class XamlVisualStateLayer : SharedUILayer
    {
        internal CalendarCellModel lastHoveredCell;
        internal FrameworkElement holdVisual;
        internal List<CalendarCellModel> cellsToUpdate;

        private const int DefaultHoldVisualZIndex = 300;

        public XamlVisualStateLayer()
        {
            this.cellsToUpdate = new List<CalendarCellModel>();
        }

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        internal void UpdateHoverDecoration(CalendarCellModel hoveredCellModel)
        {
            if (hoveredCellModel != null && hoveredCellModel == this.lastHoveredCell)
            {
                return;
            }

            this.cellsToUpdate.Clear();

            if (this.lastHoveredCell != null)
            {
                this.lastHoveredCell.IsPointerOver = false;
                this.cellsToUpdate.Add(this.lastHoveredCell);
            }

            this.lastHoveredCell = hoveredCellModel;

            if (hoveredCellModel != null)
            {
                hoveredCellModel.IsPointerOver = true;
                this.cellsToUpdate.Add(hoveredCellModel);
            }

            if (this.cellsToUpdate.Count > 0)
            {
                this.Owner.UpdatePresenters(this.cellsToUpdate);
            }
        }

        internal void UpdateHoldDecoration(Node hoveredCellModel)
        {
            this.EnsureHoldVisual();

            if (hoveredCellModel == null)
            {
                this.ClearHoldState();
            }
            else
            {
                this.ArrangeHoldVisual(hoveredCellModel);
            }
        }

        internal void ClearHoverState()
        {
            if (this.lastHoveredCell != null)
            {
                this.lastHoveredCell.IsPointerOver = false;
                this.lastHoveredCell = null;
            }
        }

        private void ClearHoldState()
        {
            this.holdVisual.Visibility = Visibility.Collapsed;
        }

        private void EnsureHoldVisual()
        {
            if (this.holdVisual == null)
            {
                this.holdVisual = new CalendarHoldClueControl();
                this.AddVisualChild(this.holdVisual);
            }
        }

        private void ArrangeHoldVisual(Node hoveredNode)
        {
            this.holdVisual.ClearValue(UIElement.VisibilityProperty);

            RadRect radRect = hoveredNode.layoutSlot;
            Rect rect = new Rect(radRect.X, radRect.Y - radRect.Height, radRect.Width, radRect.Height);

            this.holdVisual.Arrange(rect);

            Canvas.SetLeft(this.holdVisual, rect.Left + rect.Width / 3);
            Canvas.SetTop(this.holdVisual, rect.Top);
            Canvas.SetZIndex(this.holdVisual, XamlVisualStateLayer.DefaultHoldVisualZIndex);

            this.holdVisual.Height = rect.Height;
            this.holdVisual.Width = rect.Width / 3;
        }
    }
}
