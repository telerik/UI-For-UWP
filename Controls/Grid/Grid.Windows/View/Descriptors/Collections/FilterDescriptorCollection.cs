using System;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Grid.Model;

namespace Telerik.UI.Xaml.Controls.Grid
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
        internal FilterDescriptorCollection(GridModel owner)
            : base(owner)
        {
        }
    }
}
