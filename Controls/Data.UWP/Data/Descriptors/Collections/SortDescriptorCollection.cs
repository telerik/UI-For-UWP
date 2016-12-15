using System;
using System.Collections.Generic;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents a strongly-typed collection of <see cref="SortDescriptorBase"/> instances.
    /// </summary>
    public sealed class SortDescriptorCollection : DataDescriptorCollection<SortDescriptorBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortDescriptorCollection" /> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal SortDescriptorCollection(IDataDescriptorsHost owner)
            : base(owner)
        {
        }
    }
}
