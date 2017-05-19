using System;
using System.Windows.Input;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Commands;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a command that can perform a given action.
    /// </summary>
    public class DataGridFilterUICommand : ICommand
    {
        private DataGridFilteringFlyout owner;
        private DataGridFilterUIActionCommandID id;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridFilterUICommand"/> class.
        /// </summary>
        public DataGridFilterUICommand(DataGridFilteringFlyout owner, DataGridFilterUIActionCommandID id)
        {
            this.owner = owner;
            this.id = id;
        }

#pragma warning disable 0067
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// The parameter used by the command.
        /// </param>
        /// <returns>
        /// Returns a value indicating whether this command can be executed.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// The parameter used by the command.
        /// </param>
        public void Execute(object parameter)
        {
            switch (this.id)
            {
                case DataGridFilterUIActionCommandID.Cancel:
                    this.Cancel();
                    return;
                case DataGridFilterUIActionCommandID.ClearFilter:
                    this.Clear();
                    return;
                case DataGridFilterUIActionCommandID.ExpandCollapse:
                    this.ExpandCollapse();
                    return;
                case DataGridFilterUIActionCommandID.Filter:
                    this.Filter();
                    return;
            }
        }

        private void Filter()
        {
            if (this.owner.Context.AssociatedDescriptor != null)
            {
                this.owner.Owner.FilterDescriptors.Remove(this.owner.Context.AssociatedDescriptor);
            }

            FilterDescriptorBase result;
            if (this.owner.IsExpanded)
            {
                CompositeFilterDescriptor compositeDescriptor = new CompositeFilterDescriptor();
                compositeDescriptor.Operator = (LogicalOperator)this.owner.OperatorCombo.SelectedIndex;
                var firstDescriptor = this.owner.FirstFilterControl.BuildDescriptor();
                var secondDescriptor = this.owner.SecondFilterControl == null ? null : this.owner.SecondFilterControl.BuildDescriptor();

                if (firstDescriptor != null)
                {
                    compositeDescriptor.Descriptors.Add(firstDescriptor);
                }
                if (secondDescriptor != null)
                {
                    compositeDescriptor.Descriptors.Add(secondDescriptor);
                }

                result = compositeDescriptor;
            }
            else
            {
                result = this.owner.FirstFilterControl.BuildDescriptor();
            }

            FilterRequestedContext filterContext = new FilterRequestedContext();
            filterContext.Descriptor = result;
            filterContext.Column = this.owner.Context.Column;
            filterContext.IsFiltering = true;
            this.owner.Owner.CommandService.ExecuteCommand(CommandId.FilterRequested, filterContext);

            this.owner.Close();
        }

        private void Clear()
        {
            FilterRequestedContext filterContext = new FilterRequestedContext();
            filterContext.IsFiltering = false;
            filterContext.Descriptor = this.owner.Context.AssociatedDescriptor;
            filterContext.Column = this.owner.Context.Column;
            this.owner.Owner.CommandService.ExecuteCommand(CommandId.FilterRequested, filterContext);

            this.owner.Close();
        }

        private void ExpandCollapse()
        {
            if (this.owner.SecondFilterControl != null)
            {
                this.owner.IsExpanded ^= true;
                this.owner.UpdateVisualState(true);
            }
        }

        private void Cancel()
        {
            var id = this.owner.DisplayMode == FilteringFlyoutDisplayMode.Inline ? DataGridFlyoutId.FilterButton : DataGridFlyoutId.FlyoutFilterButton;
            this.owner.Owner.ContentFlyout.Hide(id);
        }
    }
}
