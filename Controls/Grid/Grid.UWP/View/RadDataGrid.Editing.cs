using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid
    {
        /// <summary>
        /// Identifies the <see cref="ExternalEditor"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ExternalEditorProperty =
            DependencyProperty.Register(nameof(ExternalEditor), typeof(IGridExternalEditor), typeof(RadDataGrid), new PropertyMetadata(new DataGridFormEditor()));

        /// <summary>
        /// Identifies the <see cref="UserEditMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty UserEditModeProperty =
            DependencyProperty.Register(nameof(UserEditMode), typeof(DataGridUserEditMode), typeof(RadDataGrid), new PropertyMetadata(DataGridUserEditMode.None, OnUserEditModeChanged));

        internal EditingService editService;

        /// <summary>
        /// Gets or sets the user edit mode of the DataGrid.
        /// </summary>
        /// <value>The can user edit.</value>
        public DataGridUserEditMode UserEditMode
        {
            get { return (DataGridUserEditMode)this.GetValue(UserEditModeProperty); }
            set { this.SetValue(UserEditModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets external editor of the DataGrid.
        /// </summary>
        public IGridExternalEditor ExternalEditor
        {
            get { return (IGridExternalEditor)GetValue(ExternalEditorProperty); }
            set { SetValue(ExternalEditorProperty, value); }
        }

        /// <summary>
        /// Begins the edit operation for the specified data item.
        /// </summary>
        /// <param name="item">The data item.</param>
        public bool BeginEdit(object item)
        {
            return this.BeginEdit(item, ActionTrigger.Programmatic, null);
        }

        /// <summary>
        /// Cancels the current edit operation.
        /// </summary>
        public bool CancelEdit()
        {
            if (!this.editService.IsEditing)
            {
                return false;
            }

            return this.CancelEdit(ActionTrigger.Programmatic, null);
        }

        /// <summary>
        /// Commits the current edit operation.
        /// </summary>
        public bool CommitEdit()
        {
            return this.CommitEdit(new DataGridCellInfo(this.CurrentItem, null), ActionTrigger.Programmatic, null);
        }

        internal bool BeginEdit(object item, ActionTrigger trigger, object parameter)
        {
            // TODO: optimize this if needed.
            var info = this.Model.FindItemInfo(item);

            if (info != null)
            {
                return this.BeginEdit(info.Value, trigger, parameter);
            }

            return false;
        }

        internal bool BeginEdit(ItemInfo info, ActionTrigger trigger, object parameter)
        {  
            var cell = this.model.CellsController.GetCellsForRow(info.Slot).First();
            var cellInfo = new DataGridCellInfo(cell);

            return this.BeginEdit(cellInfo, trigger, parameter);
        }

        internal bool BeginEdit(DataGridCellInfo cellInfo, ActionTrigger trigger, object parameter)
        {
            if ((this.editService.IsEditing && !this.CommitEdit()) || !this.CurrencyService.ChangeCurrentItem(cellInfo.RowItemInfo.Item, false, true))
            {
                return false;
            }

            var context = new EditContext(cellInfo, trigger, parameter);
            this.CommandService.ExecuteCommand(CommandId.BeginEdit, context);
            return context.IsSuccessful;
        }

        internal bool CancelEdit(ActionTrigger trigger, object parameter)
        {
            var context = new EditContext(new DataGridCellInfo(this.CurrentItem, null), trigger, parameter);
            this.CommandService.ExecuteCommand(CommandId.CancelEdit, context);

            return context.IsSuccessful;
        }

        internal bool CommitEdit(DataGridCellInfo cellInfo, ActionTrigger trigger, object parameter)
        {
            var context = new EditContext(cellInfo, trigger, parameter);
            this.CommandService.ExecuteCommand(CommandId.CommitEdit, context);

            return context.IsSuccessful;
        }

        private static void OnUserEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as RadDataGrid;
            if (grid != null)
            {
                var oldMode = (DataGridUserEditMode)e.OldValue;
                grid.CommandService.ExecuteCommand(CommandId.CancelEdit, new EditContext(new DataGridCellInfo(grid.CurrentItem, null), ActionTrigger.Programmatic, null));
            }
        }
    }
}
