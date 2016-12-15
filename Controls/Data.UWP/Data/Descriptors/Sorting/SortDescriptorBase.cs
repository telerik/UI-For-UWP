using System;
using System.Collections;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents the base class for all descriptors that define a sorting operation within a data component.
    /// </summary>
    public abstract class SortDescriptorBase : OrderedDescriptor
    {
        private IComparer comparer;

        /// <summary>
        /// Gets or sets the comparer instance.
        /// </summary>
        /// <value>The comparer.</value>
        public IComparer Comparer
        {
            get
            {
                return this.comparer;
            }
            set
            {
                if (this.comparer != value)
                {
                    this.comparer = value;
                    this.OnPropertyChanged();
                }
            }
        }

        internal override DataChangeFlags UpdateFlags
        {
            get
            {
                return DataChangeFlags.Sort;
            }
        }
    }
}