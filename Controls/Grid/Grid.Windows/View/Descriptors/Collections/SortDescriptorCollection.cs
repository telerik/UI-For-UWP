using System;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Grid.Model;

namespace Telerik.UI.Xaml.Controls.Grid
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
        internal SortDescriptorCollection(GridModel owner)
            : base(owner)
        {
        }
    }
}
