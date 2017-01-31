using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Encapsulates the data associated with the <see cref="CommandId.FlyoutGroupHeaderTap"/> command within a <see cref="RadDataGrid"/> instance.
    /// </summary>
    public class FlyoutGroupHeaderTapContext
    {
        /// <summary>
        /// Gets the <see cref="GroupDescriptorBase"/> instance associated with the command.
        /// </summary>
        public GroupDescriptorBase Descriptor
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the <see cref="DataGridFlyoutGroupHeaderTapAction"/> value that identifies the meaning of the Tap event.
        /// </summary>
        public DataGridFlyoutGroupHeaderTapAction Action
        {
            get;
            internal set;
        }
    }
}
