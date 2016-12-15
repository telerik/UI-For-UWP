using System;
using System.Collections.Generic;

namespace Telerik.Core.Data
{
    internal class DataSourceGroup : DataSourceItem, IDataSourceGroup
    {
        private List<IDataSourceItem> childItems;
        private IDataSourceGroup nextGroup;
        private bool isExpanded;
        private DataGroup dataGroup;

        public DataSourceGroup(RadListSource owner, object value) : base(owner, value)
        {
            this.childItems = new List<IDataSourceItem>();
            this.dataGroup = value as DataGroup;

            // group is expanded by default
            this.isExpanded = true;
        }

        public IDataSourceItem LastChildItem
        {
            get
            {
                int count = this.childItems.Count;
                if (count > 0)
                {
                    return this.childItems[count - 1];
                }

                return null;
            }
        }

        public int Level
        {
            get
            {
                int level = 0;
                IDataSourceGroup parentGroup = this.ParentGroup;

                while (parentGroup != null)
                {
                    level++;
                    parentGroup = parentGroup.ParentGroup;
                }

                return level;
            }
        }

        public IDataSourceGroup NextGroup
        {
            get
            {
                return this.nextGroup;
            }
            set
            {
                this.nextGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the groups is expanded (its child items are visible and may be enumerated).
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }
            set
            {
                this.isExpanded = value;
            }
        }

        /// <summary>
        /// Gets the child items for this group.
        /// </summary>
        public IList<IDataSourceItem> ChildItems
        {
            get
            {
                return this.childItems;
            }
        }

        public bool HasChildGroups
        {
            get
            {
                return this.dataGroup.HasChildGroups;
            }
        }
    }
}