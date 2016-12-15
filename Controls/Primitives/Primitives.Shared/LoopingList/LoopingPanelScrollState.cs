using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.LoopingList
{
    /// <summary>
    /// Defines the possible scrolling states a LoopingPanel instance may enter.
    /// </summary>
    internal enum LoopingPanelScrollState
    {
        /// <summary>
        /// The panel is currently not scrolling.
        /// </summary>
        NotScrolling,

        /// <summary>
        /// The panel is scrolled due to a drag manipulation.
        /// </summary>
        Scrolling,

        /// <summary>
        /// A logical index is brought into view via animated scrolling.
        /// </summary>
        ScrollingToIndex,

        /// <summary>
        /// The panel is scrolled to snap to the middle item.
        /// </summary>
        SnapScrolling
    }
}
