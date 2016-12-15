using System;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Grid.Model;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a strongly-typed collection of <see cref="GroupDescriptorBase"/> instances.
    /// </summary>
    public sealed class GroupDescriptorCollection : DataDescriptorCollection<GroupDescriptorBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupDescriptorCollection" /> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal GroupDescriptorCollection(GridModel owner)
            : base(owner)
        {
        }
    }
}
