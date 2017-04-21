using Telerik.UI.Xaml.Controls.Grid.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents the execution context of a <see cref="Telerik.UI.Xaml.Controls.Grid.Commands.ColumnHeaderActionCommand"/> command.
    /// </summary>
    public class ColumnHeaderActionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnHeaderActionContext"/> class.
        /// </summary>
        public ColumnHeaderActionContext(string key, DataGridColumnHeader columnHeader)
        {
            this.Key = key;
            this.ColumnHeader = columnHeader;
        }

        /// <summary>
        /// Gets or sets the key of the action.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the Tapped Header of the DataGrid.
        /// </summary>
        public DataGridColumnHeader ColumnHeader { get; set; }
    }
}
