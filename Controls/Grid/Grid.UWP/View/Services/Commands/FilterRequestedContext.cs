using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Encapsulates the context of a <see cref="DataGridCommand"/> associated with the <see cref="CommandId.FilterRequested"/> event.
    /// </summary>
    public class FilterRequestedContext
    {
        /// <summary>
        /// Gets a value indicating whether the Filter button has triggered the action or the ClearFilter button. If true, the Filter button has been tapped.
        /// </summary>
        public bool IsFiltering
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the <see cref="FilterDescriptorBase"/> instance associated with the context.
        /// </summary>
        public FilterDescriptorBase Descriptor
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the <see cref="DataGridColumn"/> instance that will be filtered.
        /// </summary>
        public DataGridColumn Column
        {
            get;
            internal set;
        }
    }
}
