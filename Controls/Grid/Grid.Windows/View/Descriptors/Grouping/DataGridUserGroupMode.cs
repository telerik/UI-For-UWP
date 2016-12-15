using System;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Defines the available modes that control the User Input and Experience related to the grouping operations within the <see cref="RadDataGrid"/> component.
    /// </summary>
    public enum DataGridUserGroupMode
    {
        /// <summary>
        /// The default (automatic) option is used.
        /// </summary>
        Auto,

        /// <summary>
        /// The grouping exposed to the user through the Grouping UI is enabled.
        /// </summary>
        Enabled,

        /// <summary>
        /// The grouping exposed to the user through the Grouping UI is disabled.
        /// </summary>
        Disabled
    }
}
