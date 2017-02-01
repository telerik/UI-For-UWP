using System;
using System.Collections.Generic;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal class XamlContentLayer : ContentLayer
    {
        private DataGridContentLayerPanel contentPanel = new DataGridContentLayerPanel();

        /// <summary>
        /// Gets the <see cref="UIElement" /> instance that is used by this layer.
        /// </summary>
        protected internal override UIElement VisualElement
        {
            get
            {
                return this.contentPanel;
            }
        }

        internal override bool CanProcessColumnDefinition(DataGridColumn definition)
        {
            // XAML layer should be able to process all column definitions
            return true;
        }

        internal override object GenerateCellContainer(object rowItem, DataGridColumn definition)
        {
            var container = definition.CreateContainer(rowItem) as UIElement;
            if (container != null)
            {
                this.AddVisualChild(container);
                return container;
            }

            return null;
        }

        internal override Size MeasureCell(GridCellModel cell, double availableWidth)
        {
            return cell.Column.MeasureCell(cell, availableWidth).ToSize();
        }

        internal override void ArrangeCell(GridCellModel cell)
        {
            FrameworkElement container = cell.Container as FrameworkElement;
            if (container == null)
            {
                return;
            }

            RadRect layoutSlot = cell.layoutSlot;

            var rect = this.Owner.InflateCellHorizontally(cell, layoutSlot);
            rect = this.Owner.InflateCellVertically(cell, rect);

            container.Arrange(rect.ToRect());
        }

        internal override void PrepareCell(GridCellModel cell)
        {
            cell.Column.PrepareCell(cell);
        }

        internal override void RemoveCell(GridCellModel cell)
        {
            UIElement container = cell.Container as UIElement;
            if (container != null)
            {
                this.RemoveVisualChild(container);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.contentPanel.Owner = this.Owner;
        }

        protected override void OnDetached(RadDataGrid previousOwner)
        {
            base.OnDetached(previousOwner);

            this.contentPanel.Owner = null;
        }
    }
}
