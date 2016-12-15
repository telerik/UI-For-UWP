using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class XamlGridCellGenerator : IUIContainerGenerator<GridCellModel, CellGenerationContext>
    {
        private RadDataGrid owner;

        public XamlGridCellGenerator(RadDataGrid owner)
        {
            this.owner = owner;
        }

        void IUIContainerGenerator<GridCellModel, CellGenerationContext>.PrepareContainerForItem(GridCellModel decorator)
        {
            if (this.owner.DecorationLayer != null)
            {
                this.owner.DecorationLayer.PrepareCellDecoration(decorator);
            }

            var layer = this.owner.GetContentLayerForColumn(decorator.Column);
            if (layer != null)
            {
                layer.PrepareCell(decorator);
            }
        }

        void IUIContainerGenerator<GridCellModel, CellGenerationContext>.ClearContainerForItem(GridCellModel decorator)
        {
            if (decorator.Column != null)
            {
                decorator.Column.ClearCell(decorator);
            }
            decorator.Column = null;
            decorator.parent = null;
        }

        object IUIContainerGenerator<GridCellModel, CellGenerationContext>.GetContainerTypeForItem(CellGenerationContext info)
        {
            var definition = info.ColumnItem as DataGridColumn;
            if (definition != null)
            {
                return definition.GetContainerType(info.RowItem);
            }

            // TODO: Default type?
            return null;
        }

        object IUIContainerGenerator<GridCellModel, CellGenerationContext>.GenerateContainerForItem(CellGenerationContext info, object containerType)
        {
            var definition = info.ColumnItem as DataGridColumn;
            if (definition == null)
            {
                return null;
            }

            var layer = this.owner.GetContentLayerForColumn(definition);
            if (layer != null)
            {
                var container = layer.GenerateCellContainer(info.RowItem, definition) as FrameworkElement;
                if (container != null)
                {
                    container.Tag = info.IsFrozen;
                }

                return container;
            }

            return null;
        }

        void IUIContainerGenerator<GridCellModel, CellGenerationContext>.MakeVisible(GridCellModel node)
        {
            UIElement container = node.Container as UIElement;
            if (container != null)
            {
                container.ClearValue(UIElement.VisibilityProperty);
            }

            UIElement decorationContainer = node.DecorationContainer as UIElement;
            if (decorationContainer != null)
            {
                decorationContainer.ClearValue(UIElement.VisibilityProperty);
            }
        }

        void IUIContainerGenerator<GridCellModel, CellGenerationContext>.MakeHidden(GridCellModel node)
        {
            UIElement container = node.Container as UIElement;
            if (container != null)
            {
                container.Visibility = Visibility.Collapsed;
                container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            UIElement decorationContainer = node.DecorationContainer as UIElement;
            if (decorationContainer != null)
            {
                decorationContainer.Visibility = Visibility.Collapsed;
            }
        }

        public void SetOpacity(GridCellModel decorator, byte opacity)
        {
            if (decorator != null)
            {
                UIElement container = decorator.Container as UIElement;
                if (container != null)
                {
                    container.Opacity = opacity;
                }
            }
        }
    }
}
