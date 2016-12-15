using System;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Specifies the possible group header display modes within a <see cref="RadDataGrid"/> component.
    /// </summary>
    public enum DataGridGroupHeaderDisplayMode
    {
        /// <summary>
        /// The path of group headers to the first visible data item are frozen on top of other content.
        /// </summary>
        Frozen,

        /// <summary>
        /// Group headers are scrollable together with the content.
        /// </summary>
        Scrollable,
    }
}
