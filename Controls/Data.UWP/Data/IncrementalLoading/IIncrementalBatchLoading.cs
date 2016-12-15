using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Telerik.Core.Data
{
    /// <summary>
    /// Specifies a calling contract for collection views that support incremental
    /// loading adding the notion of batch size.
    /// </summary>
    public interface IIncrementalBatchLoading : ISupportIncrementalLoading
    {
        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        /// <value>The size of the batch.</value>
        uint? BatchSize { get; set; }
    }
}