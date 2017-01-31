using System;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Defines the possible modes of the built-in User (through column header Tap) sort functionality of a <see cref="RadDataGrid"/> component.
    /// </summary>
    public enum DataGridUserSortMode
    {
        /// <summary>
        /// The mode is automatically determines based on the input Device.
        /// If the input is through Touch then the Multiple mode is used. If the input is a Mouse, then <see cref="Single"/> or <see cref="Multiple"/> is determined based on the CTRL Modifier Key.
        /// </summary>
        Auto,

        /// <summary>
        /// The user is not allowed to sort by Tapping on a column header.
        /// </summary>
        None,

        /// <summary>
        /// Singe sort descriptor is allowed. When the user Taps a column header, the SortDescriptor collection will cleared and a new descriptor will be added.
        /// </summary>
        Single,

        /// <summary>
        /// Multiple sort descriptors are allowed.
        /// </summary>
        Multiple
    }
}
