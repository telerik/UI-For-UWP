using System;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class DataBindingCompleteCommand : DataGridCommand
    {
        public override bool CanExecute(object parameter)
        {
            if (this.Owner == null)
            {
                return false;
            }

            return parameter is DataBindingCompleteEventArgs;
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            this.Owner.OnDataBindingComplete(parameter as DataBindingCompleteEventArgs);
        }
    }
}
