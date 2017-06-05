using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class CellFlyoutActionCommand : DataGridCommand
    {
        public override bool CanExecute(object parameter)
        {
            return parameter is CellFlyoutActionContext;
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            var context = parameter as CellFlyoutActionContext;
            var cell = context.CellInfo.Cell;
            var column = cell.Column;

            if (!context.IsOpen)
            {
                this.Owner.ContentFlyout.Hide(DataGridFlyoutId.Cell);
            }
            else
            {
                FrameworkElement flyoutContent = null;

                if (context.FlyoutTemplate != null)
                {
                    flyoutContent = context.FlyoutTemplate.LoadContent() as FrameworkElement;
                }

                if (flyoutContent == null)
                {
                    DataGridTextColumn textColumn = column as DataGridTextColumn;

                    if (textColumn == null)
                    {
                        return;
                    }

                    flyoutContent = column.CreateContainer(context.CellInfo.Item) as FrameworkElement;

                    column.PrepareCell(new GridCellModel() { Container = flyoutContent, ContainerType = flyoutContent.GetType(), Column = column, Value = cell.Value });

                    flyoutContent.Style = textColumn.DefaultCellFlyoutContentStyle;
                }

                DataGridCellFlyoutControl container = new DataGridCellFlyoutControl();
                container.DataContext = cell;
                container.Child = flyoutContent;

                container.Width = cell.layoutSlot.Width;
                container.MinHeight = cell.layoutSlot.Height;
                this.Owner.ContentFlyout.Show(DataGridFlyoutId.Cell, container, context.Gesture == View.CellFlyoutGesture.PointerOver);        
            }
        }
    }
}
