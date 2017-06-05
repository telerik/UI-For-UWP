using System.Collections.Generic;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Used to group items, provide well known groups, sort and filter the groups.
    /// </summary>
    internal abstract class GroupDescription : GroupDescriptionBase
    {
        private GroupFilter groupFilter;
        private bool showGroupsWithNoData;

        internal GroupDescription()
        {
        }

        /// <summary>
        /// Gets the number of levels that this instance will generate.
        /// </summary>
        /// <value>The number of levels.</value>
        public virtual int LevelCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether well known groups should be created even if there are no items for them.
        /// <example>Grouping by days may require groups for the empty days in the current month.</example>
        /// <example>Grouping by persons may require groups all persons even if they do not contain any items within the current context.</example>
        /// </summary>
        public bool ShowGroupsWithNoData
        {
            get
            {
                return this.showGroupsWithNoData;
            }

            set
            {
                if (this.showGroupsWithNoData != value)
                {
                    this.showGroupsWithNoData = value;
                    this.OnPropertyChanged(nameof(this.ShowGroupsWithNoData));
                    this.NotifyChange(new SettingsChangedEventArgs());
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="GroupFilter"/> implementation for this instance that would be used to filter the groups.
        /// </summary>
        public GroupFilter GroupFilter
        {
            get
            {
                return this.groupFilter;
            }

            set
            {
                if (this.groupFilter != value)
                {
                    this.ChangeSettingsProperty(ref this.groupFilter, value);
                    this.OnPropertyChanged(nameof(GroupFilter));
                    this.NotifyChange(new SettingsChangedEventArgs());
                }
            }
        }

        /// <summary>
        /// Returns all possible group keys for this instance.
        /// </summary>
        /// <param name="uniqueNames">Enumeration of all unique group keys that were discovered after grouping.</param>
        /// <param name="parentGroupNames">Enumeration of all parent groups.</param>
        /// <returns>The possible group keys for this instance.</returns>
        protected internal virtual IEnumerable<object> GetAllNames(IEnumerable<object> uniqueNames, IEnumerable<object> parentGroupNames)
        {
            return uniqueNames;
        }

        /// <inheritdoc />
        protected override void CloneCore(Cloneable source)
        {
            GroupDescription dg = source as GroupDescription;
            if (dg != null)
            {
                this.showGroupsWithNoData = dg.showGroupsWithNoData;
                this.groupFilter = Cloneable.CloneOrDefault(dg.groupFilter);
            }

            base.CloneCore(source);
        }
    }
}