using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Contains information about the <see cref="RadExpanderControl.ExpandedStateChanging"/> event.
    /// </summary>
    public class ExpandedStateChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandedStateChangingEventArgs" /> class.
        /// </summary>
        /// <param name="isExpanding">A boolean value determining whether the <see cref="RadExpanderControl"/> is expanding or collapsing.</param>
        public ExpandedStateChangingEventArgs(bool isExpanding)
        {
            this.IsExpanding = isExpanding;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="RadExpanderControl"/> is being expanded or collapsed.
        /// </summary>
        public bool IsExpanding
        {
            get;
            private set;
        }
    }
}
