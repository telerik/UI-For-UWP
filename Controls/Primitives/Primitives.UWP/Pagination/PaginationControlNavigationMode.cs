using System;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines the modes that allow to choose which item should be marked as current.
    /// </summary>
    public enum PaginationControlNavigationMode
    {
        /// <summary>
        /// In this mode, the item that is tapped becomes the current item. This is the default mode.
        /// </summary>
        Direct,

        /// <summary>
        /// In this mode, the next or the previous item becomes current, depending on whether the new tapped item is respectively after or before the current item.
        /// </summary>
        Adjacent
    }
}
