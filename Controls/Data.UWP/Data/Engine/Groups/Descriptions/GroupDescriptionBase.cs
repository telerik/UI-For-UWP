using System.ComponentModel;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Base class for GroupDescription.
    /// </summary>
    internal abstract class GroupDescriptionBase : DescriptionBase, IGroupDescription
    {
        private SortOrder sortOrder;
        private GroupComparer groupComparer;

        internal GroupDescriptionBase()
        {
            this.groupComparer = new GroupNameComparer();
        }

        /// <summary>
        /// Gets or sets the <see cref="Telerik.Data.Core.GroupComparer"/> that will be used for group comparisons.
        /// </summary>
        /// <value>A <see cref="Telerik.Data.Core.GroupComparer"/> implementation.</value>
        GroupComparer IGroupDescription.GroupComparer
        {
            get
            {
                return this.groupComparer;
            }

            set
            {
                if (this.groupComparer != value)
                {
                    this.ChangeSettingsProperty(ref this.groupComparer, value);
                    this.OnPropertyChanged(nameof(Telerik.Data.Core.GroupComparer));
                    this.NotifyChange(new SettingsChangedEventArgs());
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="SortOrder"/> implementation used to sort the groups created by this instance.
        /// </summary>
        public SortOrder SortOrder
        {
            get
            {
                return this.sortOrder;
            }

            set
            {
                if (this.sortOrder != value)
                {
                    this.sortOrder = value;
                    this.OnPropertyChanged(nameof(SortOrder));
                    this.NotifyChange(new SettingsChangedEventArgs());
                }
            }
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
            base.CloneCore(source);

            GroupDescriptionBase original = source as GroupDescriptionBase;
            if (original != null)
            {
                this.SortOrder = original.SortOrder;
                ((IGroupDescription)this).GroupComparer = Cloneable.CloneOrDefault(((IGroupDescription)original).GroupComparer);
            }
        }
    }
}