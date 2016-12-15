using System;
using System.Collections.Generic;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents a strongly-typed collection of <see cref="FilterDescriptorBase"/> instances.
    /// </summary>
    public sealed class FilterDescriptorCollection : DataDescriptorCollection<FilterDescriptorBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterDescriptorCollection" /> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal FilterDescriptorCollection(IDataDescriptorsHost owner)
            : base(owner)
        {
        }
    }
}
