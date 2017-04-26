namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Represents an ActionContext class.
    /// </summary>
    public class ActionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionContext"/> class.
        /// </summary>
        public ActionContext(DataGridColumn column, ColumnHeaderFlyoutCommand command)
        {
            this.Column = column;
            this.Command = command;
            this.CanSort = this.Column.CanUserSort && this.Column.HeaderControl.Owner.UserSortMode != DataGridUserSortMode.None;
            this.CanGroup = this.Column.CanUserGroup && this.Column.HeaderControl.Owner.UserGroupMode != DataGridUserGroupMode.Disabled;
            this.CanFilter = this.Column.CanUserFilter && this.Column.HeaderControl.Owner.UserFilterMode != DataGridUserFilterMode.Disabled;
        }
        
        /// <summary>
        /// Gets or sets the Column that uses the action context.
        /// </summary>
        public DataGridColumn Column { get; set; }

        /// <summary>
        /// Gets or sets the Command used by the ColumnHeader.
        /// </summary>
        public ColumnHeaderFlyoutCommand Command { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether information if column can be sorted.
        /// </summary>
        public bool CanSort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether column can be grouped.
        /// </summary>
        public bool CanGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether column can be filtered.
        /// </summary>
        public bool CanFilter { get; set; }
    }
}
