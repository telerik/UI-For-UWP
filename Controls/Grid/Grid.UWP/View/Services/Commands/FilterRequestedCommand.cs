using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class FilterRequestedCommand : DataGridCommand
    {
        public override bool CanExecute(object parameter)
        {
            return this.Owner != null && parameter is FilterRequestedContext;
        }
        public override void Execute(object parameter)
        {
            var context = parameter as FilterRequestedContext;
            if (context.IsFiltering)
            {
                this.ExecuteFilter(context);
            }
            else
            {
                this.ExecuteClearFilter(context);
            }           
        }

        private void ExecuteFilter(FilterRequestedContext context)
        {
            if (context != null)
            {
                if (context.Descriptor != null)
                {
                    this.Owner.FilterDescriptors.Add(context.Descriptor);
                    if (context.Descriptor.DescriptorPeer != context.Column)
                    {
                        context.Descriptor.UpdateAssociatedPeer(context.Column);
                    }
                }
            }
        }

        private void ExecuteClearFilter(FilterRequestedContext context)
        {
            if (context.Descriptor != null)
            {
                this.Owner.FilterDescriptors.Remove(context.Descriptor);
            }
        }
    }
}
