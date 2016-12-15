using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.Common;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class XamlGridEditRowGenerator : IUIContainerGenerator<GridEditRowModel, RowGenerationContext>
    {
        private static Type editRowType = typeof(DataGridEditRow);

        private RadDataGrid owner;

        public XamlGridEditRowGenerator(RadDataGrid owner)
        {
            this.owner = owner;
        }

        public void PrepareContainerForItem(GridEditRowModel decorator)
        {
            if (decorator != null)
            {
                var element = decorator.Container as DataGridEditRow;
                if (element != null)
                {
                    element.DataContext = decorator.ItemInfo.Item;

                    var isCancelButtonVisibile = (decorator.IsFrozen && element.Owner.FrozenColumnCount > 0) || (!decorator.IsFrozen && element.Owner.FrozenColumnCount < 1);
                    element.UpdateCloseButtonVisibility(isCancelButtonVisibile);
                }
            }
        }

        public void ClearContainerForItem(GridEditRowModel decorator)
        {
            if (decorator != null)
            {
                var element = decorator.Container as DataGridEditRow;
                if (element != null)
                 {
                    element.ClearValue(FrameworkElement.DataContextProperty);
                    element.UpdateCloseButtonVisibility(false);

                    if (decorator.IsFrozen)
                    {
                        this.owner.FrozenEditRowLayer.EditorLayoutSlots.Clear();
                    }
                    else
                    {
                        this.owner.EditRowLayer.EditorLayoutSlots.Clear();
                    }       
                }
            }
        }

        public object GetContainerTypeForItem(RowGenerationContext context)
        {
            return editRowType;
        }

        public object GenerateContainerForItem(RowGenerationContext context, object containerType)
        {
            DataGridEditRow child = null;

            if (containerType == editRowType)
            {
                child = new DataGridEditRow();
                child.Owner = this.owner;

                if (context.IsFrozen)
                {
                    this.owner.FrozenEditRowLayer.AddVisualChild(child);
                    this.owner.FrozenEditRowLayer.AddVisualChild(child.CancelButton);
                }
                else
                {
                    this.owner.EditRowLayer.AddVisualChild(child);
                    this.owner.EditRowLayer.AddVisualChild(child.CancelButton);
                }
            }

            return child;
        }

        public void MakeVisible(GridEditRowModel decorator)
        {
            if (decorator != null)
            {
                UIElement container = decorator.Container as UIElement;
                if (container != null)
                {
                    container.ClearValue(UIElement.VisibilityProperty);
                }
            }
        }

        public void MakeHidden(GridEditRowModel decorator)
        {
            if (decorator != null)
            {
                UIElement container = decorator.Container as UIElement;
                if (container != null)
                {
                    container.Visibility = Visibility.Collapsed;
                    container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
            }
        }

        public void SetOpacity(GridEditRowModel decorator, byte opacity)
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
