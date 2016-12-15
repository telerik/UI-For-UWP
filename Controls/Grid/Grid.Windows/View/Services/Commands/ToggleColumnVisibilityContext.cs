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

        public bool IsColumnVisible
        {
            get;
            set;
        }
    }
}
