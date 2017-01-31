using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents the base class for all descriptors that define a sorting operation within a <see cref="RadDataGrid"/> component.
    /// </summary>
    public abstract class SortDescriptorBase : OrderedDescriptor
    {
        internal override DataChangeFlags UpdateFlags
        {
            get
            {
                return DataChangeFlags.Sort;
            }
        }
    }
}
