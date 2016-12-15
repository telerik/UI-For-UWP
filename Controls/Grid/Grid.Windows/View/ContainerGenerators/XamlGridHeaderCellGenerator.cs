using System;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class XamlGridHeaderCellGenerator : IUIContainerGenerator<GridHeaderCellModel, ColumnGenerationContext>
    {
        private static Type columnHeaderType = typeof(DataGridColumnHeader);

        private RadDataGrid owner;

        public XamlGridHeaderCellGenerator(RadDataGrid owner)
        {
            this.owner = owner;
        }

        public void PrepareContainerForItem(GridHeaderCellModel decorator)
        {
            decorator.Column = decorator.ItemInfo.Item as DataGridColumn;
            decorator.Column.PrepareColumnHeaderCell(decorator);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void ClearContainerForItem(GridHeaderCellModel decorator)
        {
            decorator.Column.ClearColumnHeaderCell(decorator);
            decorator.Column = null;
        }

        public object GetContainerTypeForItem(ColumnGenerationContext info)
        {
            return columnHeaderType;
        }

        public object GenerateContainerForItem(ColumnGenerationContext info, object containerType)
        {
            var header = new DataGridColumnHeader();

            if (info.IsFrozen)
            {
                // TODO: handle drag and drop
                this.owner.FrozenColumnHeadersHost.Children.Add(header);
            }
            else
            {
                this.owner.ColumnHeadersHost.AddChild(header);
            }

            return header;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void MakeVisible(GridHeaderCellModel decorator)
        {
            UIElement container = decorator.Container as UIElement;
            if (container != null)
            {
                container.ClearValue(UIElement.VisibilityProperty);
            }

            UIElement decorationContainer = decorator.GetDecorationContainer() as UIElement;
            if (decorationContainer != null)
            {
                decorationContainer.ClearValue(UIElement.VisibilityProperty);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void MakeHidden(GridHeaderCellModel decorator)
        {
            UIElement container = decorator.Container as UIElement;
            if (container != null)
            {
                container.Visibility = Visibility.Collapsed;
                container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            UIElement decorationContainer = decorator.DecorationContainer as UIElement;
            if (decorationContainer != null)
            {
                decorationContainer.Visibility = Visibility.Collapsed;
            }
        }

        public void SetOpacity(GridHeaderCellModel decorator, byte opacity)
        {
            throw new NotImplementedException();
        }
    }
}
