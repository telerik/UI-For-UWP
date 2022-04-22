using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class XamlGridEditCellGenerator : IUIContainerGenerator<GridCellEditorModel, CellGenerationContext>
    {
        private RadDataGrid owner;

        public XamlGridEditCellGenerator(RadDataGrid owner)
        {
            this.owner = owner;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void PrepareContainerForItem(GridCellEditorModel decorator)
        {
            var column = decorator.Column;

            if (column != null)
            {
                column.PrepareEditorContainer(decorator);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void ClearContainerForItem(GridCellEditorModel decorator)
        {
            var column = decorator.Column;

            if (column != null)
            {
                column.ClearEditorContainer(decorator);
            }
        }

        public object GetContainerTypeForItem(CellGenerationContext info)
        {
            var column = info.ColumnItem as DataGridColumn;

            if (column != null)
            {
                return column.GetEditorType(info.RowItem);
            }

            return null;
        }

        public object GenerateContainerForItem(CellGenerationContext info, object containerType)
        {
            var column = info.ColumnItem as DataGridColumn;
            object container = null;
            if (column != null)
            {
                container = column.CreateEditorContainer(info.RowItem, containerType);
                var pair = container as Tuple<FrameworkElement, FrameworkElement, FrameworkElement>;

                if (pair != null)
                {
                    if (info.IsFrozen)
                    {
                        this.owner.FrozenEditRowLayer.AddVisualChild(pair.Item2);
                    }
                    else
                    {
                        this.owner.EditRowLayer.AddVisualChild(pair.Item2);
                    }
                }
                else
                {
                    var element = container as UIElement;

                    if (element != null)
                    {
                        if (info.IsFrozen)
                        {
                            this.owner.FrozenEditRowLayer.AddVisualChild(element);
                        }
                        else
                        {
                            this.owner.EditRowLayer.AddVisualChild(element);
                        }
                    }
                }
            }

            return container;
        }

        public void MakeVisible(GridCellEditorModel decorator)
        {
            var pair = decorator.Container as Tuple<FrameworkElement, FrameworkElement, FrameworkElement>;

            if (pair != null && pair.Item2 != null)
            {
                pair.Item2.ClearValue(UIElement.VisibilityProperty);
            }
            else
            {
                var element = decorator.Container as UIElement;

                if (element != null)
                {
                    element.ClearValue(UIElement.VisibilityProperty);
                }
            }

            UIElement decorationContainer = decorator.DecorationContainer as UIElement;
            if (decorationContainer != null)
            {
                decorationContainer.ClearValue(UIElement.VisibilityProperty);
            }
        }

        public void MakeHidden(GridCellEditorModel decorator)
        {
            var pair = decorator.Container as Tuple<FrameworkElement, FrameworkElement, FrameworkElement>;

            if (pair != null && pair.Item2 != null)
            {
                pair.Item2.Visibility = Visibility.Collapsed;
                pair.Item2.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            else
            {
                var element = decorator.Container as UIElement;

                if (element != null)
                {
                    element.Visibility = Visibility.Collapsed;
                    element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
            }

            UIElement decorationContainer = decorator.DecorationContainer as UIElement;
            if (decorationContainer != null)
            {
                decorationContainer.Visibility = Visibility.Collapsed;
            }
        }

        public void SetOpacity(GridCellEditorModel decorator, byte opacity)
        {
            if (decorator != null)
            {
                var pair = decorator.Container as Tuple<FrameworkElement, FrameworkElement, FrameworkElement>;

                if (pair != null && pair.Item2 != null)
                {
                    pair.Item2.Opacity = opacity;
                }
            }
        }
    }
}
