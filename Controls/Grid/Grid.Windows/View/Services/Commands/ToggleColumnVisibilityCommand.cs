using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class ToggleColumnVisibilityCommand : DataGridCommand
    {
        public override bool CanExecute(object parameter)
        {
            return this.Owner != null && parameter is ToggleColumnVisibilityContext;
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            ToggleColumnVisibilityContext context = parameter as ToggleColumnVisibilityContext;

            context.Column.IsVisible = context.IsColumnVisible;
        }
    }
}
