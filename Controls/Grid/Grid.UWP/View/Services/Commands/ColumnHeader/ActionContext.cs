using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    public class ActionContext
    {
        public ActionContext(DataGridColumn column, ColumnHeaderFlyoutCommand command)
        {
            this.Column = column;
            this.Command = command;
            this.CanSort = this.Column.CanUserSort && this.Column.HeaderControl.Owner.UserSortMode != DataGridUserSortMode.None;
            this.CanGroup = this.Column.CanUserGroup && this.Column.HeaderControl.Owner.UserGroupMode != DataGridUserGroupMode.Disabled;
            this.CanFilter = this.Column.CanUserFilter && this.Column.HeaderControl.Owner.UserFilterMode != DataGridUserFilterMode.Disabled;
        }

        public DataGridColumn Column { get; set; }
        public ColumnHeaderFlyoutCommand Command { get; set; }
        public bool CanSort { get; set; }
        public bool CanGroup { get; set; }
        public bool CanFilter { get; set; }
    }
}
