using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class FlyoutGroupHeaderTapCommand : DataGridCommand
    {
        public override bool CanExecute(object parameter)
        {
            return parameter is FlyoutGroupHeaderTapContext;
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            var context = parameter as FlyoutGroupHeaderTapContext;
            switch (context.Action)
            {
                case DataGridFlyoutGroupHeaderTapAction.ChangeSortOrder:
                    // toggle sort order
                    if (context.Descriptor.SortOrder == SortOrder.Ascending)
                    {
                        context.Descriptor.SortOrder = SortOrder.Descending;
                    }
                    else
                    {
                        context.Descriptor.SortOrder = SortOrder.Ascending;
                    }
                    break;
                case DataGridFlyoutGroupHeaderTapAction.RemoveDescriptor:
                    this.Owner.GroupDescriptors.Remove(context.Descriptor);
                    break;
            }
        }
    }
}
