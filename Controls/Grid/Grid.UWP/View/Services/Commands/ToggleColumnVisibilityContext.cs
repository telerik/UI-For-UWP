using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Represents the execution context of a <see cref="CommandId.ToggleColumnVisibility"/> command.
    /// </summary>
    public class ToggleColumnVisibilityContext
    {
        /// <summary>
        /// Gets the <see cref="DataGridColumn"/> instance that is being shown or hidden.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the visibility of the Column. When set to true the Column is Visible. When set to false it is Collapsed.
        /// </summary>
        public bool IsColumnVisible
        {
            get;
            set;
        }
    }
}
