using System;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Defines the possible actions that may occur after the Tap event over a group header control.
    /// </summary>
    public enum DataGridFlyoutGroupHeaderTapAction
    {
        /// <summary>
        /// The Tap event is interpreted as a change in the sort order property of the underlying <see cref="Telerik.Data.Core.GroupDescriptorBase"/> object.
        /// </summary>
        ChangeSortOrder,

        /// <summary>
        /// The Tap event is interpreted as a removal of the underlying <see cref="Telerik.Data.Core.GroupDescriptorBase"/> object
        /// from the GroupDescriptors collection in the owning <see cref="RadDataGrid"/> instance.
        /// </summary>
        RemoveDescriptor
    }
}