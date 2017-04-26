using Telerik.Core;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Primitives
{
    /// <summary>
    /// Represents the context that is passed to a <see cref="ListViewGroupHeader"/> control that represents a <see cref="IDataGroup"/> within a <see cref="RadListView"/> instance.
    /// </summary>
    public class GroupHeaderContext : ViewModelBase
    {
        private GroupDescriptorBase descriptor;
        private IDataGroup group;
        private bool isExpanded;

        /// <summary>
        /// Gets the <see cref="RadListView"/> instance that provides the grouping context.
        /// </summary>
        public RadListView Owner
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the zero-based level (or the depth) of the group.
        /// </summary>
        public int Level
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the <see cref="GroupDescriptorBase"/> instance that provides the grouping information.
        /// </summary>
        public GroupDescriptorBase Descriptor
        {
            get
            {
                return this.descriptor;
            }
            internal set
            {
                if (this.descriptor == value)
                {
                    return;
                }

                this.descriptor = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the <see cref="IDataGroup"/> instance associated with the context.
        /// </summary>
        public IDataGroup Group
        {
            get
            {
                return this.group;
            }
            internal set
            {
                if (this.group == value)
                {
                    return;
                }

                this.group = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="IDataGroup"/>, this context is associated with, is currently expanded (has its child items visible).
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }
            set
            {
                if (this.isExpanded == value)
                {
                    return;
                }

                this.isExpanded = value;
                this.OnPropertyChanged();
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
            if (this.descriptor == null || this.group == null)
            {
                return base.ToString();
            }

            return this.descriptor.ToString() + ": " + this.group.ToString();
        }
    }
}
