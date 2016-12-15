using System;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.View;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class GroupHeaderTapCommand : DataGridCommand
    {
        public override bool CanExecute(object parameter)
        {
            return this.Owner != null && parameter is GroupHeaderContext;
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            if (this.Owner.editService.IsEditing && !this.Owner.CancelEdit())
            {
                return;
            }

            var context = parameter as GroupHeaderContext;

            // Toggle the IsExpanded state
            context.IsExpanded = !context.IsExpanded;

            this.Owner.Model.OnRowGroupExpandStateChanged(context.Group, context.IsExpanded);
        }
    }
}
