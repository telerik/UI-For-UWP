using System;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Defines the possible ways a <see cref="DataGridColumn"/> may be sized within a <see cref="RadDataGrid"/> instance.
    /// </summary>
    public enum DataGridColumnSizeMode
    {
        /// <summary>
        /// The column is stretched to the available width proportionally to its desired width.
        /// </summary>
        Stretch,

        /// <summary>
        /// The columns is sized to its desired width. That is the maximum desired width of all associated cells.
        /// </summary>
        Auto,

        /// <summary>
        /// The column has a fixed width as defined by its Width property.
        /// </summary>
        Fixed
    }
}
