using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents the base class for all descriptors that define a grouping operation within a data instance.
    /// </summary>
    public abstract class GroupDescriptorBase : OrderedDescriptor
    {
        private object displayContent;

        /// <summary>
        /// Gets or sets the content that is used to represent the group descriptor visually - for example within the grouping panel.
        /// </summary>
        public object DisplayContent
        {
            get
            {
                if (this.displayContent != null)
                {
                    return this.displayContent;
                }

                return this.ToString();
            }
            set
            {
                if (this.displayContent == value)
                {
                    return;
                }

                this.displayContent = value;
                this.OnPropertyChanged();
            }
        }

        internal override DataChangeFlags UpdateFlags
        {
            get
            {
                return DataChangeFlags.Group;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.displayContent != null)
            {
                return this.displayContent.ToString();
            }

            return base.ToString();
        }
    }
}