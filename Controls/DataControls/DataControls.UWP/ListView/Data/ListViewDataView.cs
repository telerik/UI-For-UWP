using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Telerik.Data.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Data.ListView;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class ListViewDataView : IDataViewCollection
    {
        private static readonly IReadOnlyList<object> Empty = new object[0];

        private readonly RadListView owner;

        public ListViewDataView(RadListView owner)
        {
            this.owner = owner;
        }

        public bool IsDataReady
        {
            get
            {
                return this.owner.Model.IsDataReady;
            }
        }

        public IReadOnlyList<object> Items
        {
            get
            {
                var group = this.owner.Model.CurrentDataProvider.Results.Root.RowGroup;

                if (group == null)
                {
                    return Empty;
                }

                return group.Items;
            }
        }

        public object CurrentItem
        {
            get
            {
                return this.owner.currencyService.CurrentItem;
            }
        }

        public bool IsCurrentItemInView
        {
            get
            {
                return this.owner.currencyService.IsCurrentItemInView;
            }
        }

        public IDataGroup GetParentGroup(object item)
        {
            var groups = this.EnumerateDataGroups();

            return groups.FirstOrDefault(group =>
                group.IsBottomLevel && group.Items.Contains(item));
        }

        public IEnumerable<IDataGroup> GetGroups(Predicate<IDataGroup> condition = null)
        {
            var groups = this.EnumerateDataGroups();

            if (condition == null)
            {
                return groups;
            }

            return groups.Where(group => condition(group));
        }

        public bool GetIsExpanded(IDataGroup group)
        {
            return !this.owner.Model.layoutController.Layout.IsCollapsed(group);
        }

        public void ExpandGroup(IDataGroup group)
        {
            this.owner.Model.layoutController.Layout.Expand(group);
            this.owner.updateService.RegisterUpdate((int)UpdateFlags.AllButData);
        }

        public void CollapseGroup(IDataGroup group)
        {
            this.owner.Model.layoutController.Layout.Collapse(group);
            this.owner.updateService.RegisterUpdate((int)UpdateFlags.AllButData);
        }

        public void ExpandItem(object item)
        {
            var group = this.GetParentGroup(item);

            if (group != null)
            {
                this.ExpandGroup(group);
            }
        }

        public void CollapseItem(object item)
        {
            var group = this.GetParentGroup(item);

            if (group != null)
            {
                this.CollapseGroup(group);
            }
        }

        public void ExpandAll()
        {
            foreach (var group in this.EnumerateDataGroups())
            {
                this.owner.Model.layoutController.Layout.Expand(group);
            }

            this.owner.updateService.RegisterUpdate((int)UpdateFlags.AllButData);
        }

        public void CollapseAll()
        {
            foreach (var group in this.EnumerateDataGroups())
            {
                this.owner.Model.layoutController.Layout.Collapse(group);
            }

            this.owner.updateService.RegisterUpdate((int)UpdateFlags.AllButData);
        }

        public IEnumerator GetEnumerator()
        {
            var items = this.EnumerateDataItems();

            return items.GetEnumerator();
        }

        public bool MoveCurrentTo(object item)
        {
            return this.owner.currencyService.MoveCurrentTo(item);
        }

        public bool MoveCurrentToFirst()
        {
            return this.owner.currencyService.MoveCurrentToFirst();
        }

        public bool MoveCurrentToLast()
        {
            return this.owner.currencyService.MoveCurrentToLast();
        }

        public bool MoveCurrentToNext()
        {
            return this.owner.currencyService.MoveCurrentToNext();
        }

        public bool MoveCurrentToPrevious()
        {
            return this.owner.currencyService.MoveCurrentToPrevious();
        }

        private static IEnumerable<DataGroup> EnumerateDataGroups(DataGroup group)
        {
            if (!group.IsBottomLevel)
            {
                foreach (DataGroup parent in group.Items)
                {
                    yield return parent;

                    var children = EnumerateDataGroups(parent);

                    foreach (var child in children)
                    {
                        yield return child;
                    }
                }
            }
        }

        private static IEnumerable<object> EnumerateDataItems(DataGroup group)
        {
            if (group.IsBottomLevel)
            {
                foreach (var item in group.Items)
                {
                    yield return item;
                }
            }
            else
            {
                foreach (DataGroup parent in group.Items)
                {
                    yield return parent;

                    var children = EnumerateDataItems(parent);

                    foreach (var child in children)
                    {
                        yield return child;
                    }
                }
            }
        }

        private IEnumerable<DataGroup> EnumerateDataGroups()
        {
            var group = this.owner.Model.CurrentDataProvider.Results.Root.RowGroup;

            if (group == null)
            {
                return Enumerable.Empty<DataGroup>();
            }

            return EnumerateDataGroups((DataGroup)group);
        }

        private IEnumerable<object> EnumerateDataItems()
        {
            var group = this.owner.Model.CurrentDataProvider.Results.Root.RowGroup;

            if (group == null)
            {
                return Enumerable.Empty<object>();
            }

            return EnumerateDataItems((DataGroup)group);
        }
    }
}
