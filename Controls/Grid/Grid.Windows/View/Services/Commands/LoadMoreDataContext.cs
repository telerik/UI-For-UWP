using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Represents a context, passed to a <see cref="CommandId.LoadMoreData"/> command.
    /// </summary>
    public class LoadMoreDataContext
    {
        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        /// <value>The size of the batch.</value>
        public uint? BatchSize { get; set; }
    }
}
