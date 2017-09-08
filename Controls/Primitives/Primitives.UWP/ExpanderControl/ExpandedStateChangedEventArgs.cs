using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.UI.Xaml.Controls.Primitives
{
        /// <summary>
    /// Contains information about the <see cref="RadExpanderControl.ExpandedStateChanged"/> event.
    /// </summary>
    public class ExpandedStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandedStateChangedEventArgs" /> class.
        /// </summary>
        /// <param name="isExpanded">A boolean value determining whether the <see cref="RadExpanderControl"/> is expanding or collapsing.</param>
        public ExpandedStateChangedEventArgs(bool isExpanded)
        {
            this.IsExpanded = isExpanded;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="RadExpanderControl"/> is expanded or collapsed.
        /// </summary>
        public bool IsExpanded
        {
            get;
            private set;
        }
    }
}
