using System;
using System.Collections.Specialized;
using System.Reflection;
using Telerik.Core.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class DataBoundListBoxListSourceItemFactory : IDataSourceItemFactory
    {
        internal PropertyInfo itemCheckedPropInfo;
        internal bool? sourceItemCheckedPathWritable = null;
        private RadDataBoundListBox owner;

        internal DataBoundListBoxListSourceItemFactory(RadDataBoundListBox owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Creates a group item for the specified data group.
        /// </summary>
        public IDataSourceGroup CreateGroup(RadListSource owner, DataGroup dataGroup)
        {
            return new DataSourceGroup(owner, dataGroup);
        }

        /// <summary>
        /// Creates a <see cref="T:Telerik.Core.Data.IDataSourceItem"/> instance.
        /// </summary>
        public IDataSourceItem CreateItem(RadListSource owner, object value)
        {
            DataSourceItem newItem = new DataSourceItem(owner, value);

            if (!string.IsNullOrEmpty(this.owner.itemCheckedPathCache))
            {
                this.InitializeItemCheckedState(newItem);
            }

            return newItem;
        }

        /// <summary>
        /// Raises the <see cref="E:OwningListSourceCollectionChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        public virtual void OnOwningListSourceCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.owner != null && args.Action == NotifyCollectionChangedAction.Reset)
            {
                this.owner.CheckedItems.ClearSilently(false);
            }
        }

        /// <summary>
        /// Synchronizes the IsChecked state of the <see cref="DataSourceItem"/> instance
        /// with the value of the property on the
        /// source object defined by the <see cref="RadDataBoundListBox.ItemCheckedPath"/>.
        /// </summary>
        internal void InitializeItemCheckedState(DataSourceItem item)
        {
            bool isChecked = this.GetIsCheckedValue(item.Value);

            CheckedItemsCollection<object> checkedItems = this.owner.CheckedItems;

            if (isChecked)
            {
                checkedItems.CheckItemSilently(item, true, false);
            }
            else if (item.isChecked)
            {
                checkedItems.UncheckItemSilently(item, true, false);
            }
        }

        /// <summary>
        /// Synchronizes the value of the property on the source object defined by 
        /// the <see cref="RadDataBoundListBox.ItemCheckedPath"/>
        /// with the current value of the <see cref="DataSourceItem"/> item.
        /// </summary>
        internal void UpdateSourceItemIsCheckedProperty(DataSourceItem item)
        {
            if (string.IsNullOrEmpty(this.owner.itemCheckedPathCache) || item is DataSourceGroup)
            {
                return;
            }

            object target = item.Value;
            this.owner.isInternalCheckModeSync = true;
            if (this.itemCheckedPropInfo == null)
            {
                // TODO: evaluate the type of the property,as well as whether it is readonly and make a decision how to handle the case
                this.itemCheckedPropInfo = this.GetTargetCheckedPropertyInfo(target);
            }

            if (this.itemCheckedPropInfo != null)
            {
                this.itemCheckedPropInfo.SetValue(target, item.isChecked, new object[] { });
            }
            this.owner.isInternalCheckModeSync = false;
        }

        private bool GetIsCheckedValue(object sourceItem)
        {
            bool? result = false;

            if (this.itemCheckedPropInfo == null)
            {
                this.itemCheckedPropInfo = this.GetTargetCheckedPropertyInfo(sourceItem);
            }

            if (this.itemCheckedPropInfo != null)
            {
                result = this.itemCheckedPropInfo.GetValue(sourceItem, new object[] { }) as bool?;
            }

            return result.HasValue ? result.Value : false;
        }

        private PropertyInfo GetTargetCheckedPropertyInfo(object source)
        {
            var props = source.GetType().GetRuntimeProperties();
            PropertyInfo foundInfo = null;
            foreach (PropertyInfo info in props)
            {
                if (info.Name.Equals(this.owner.itemCheckedPathCache))
                {
                    this.ValidateTargetCheckedPropertyInfo(info);
                    this.sourceItemCheckedPathWritable = info.CanWrite;
                    foundInfo = info;
                    break;
                }
            }

            if (foundInfo == null)
            {
                throw new ArgumentException("ItemCheckedPath does not point to a valid source object property. The source object property must not be static and must be public.");
            }

            return foundInfo;
        }

        private void ValidateTargetCheckedPropertyInfo(PropertyInfo info)
        {
            Type propertyType = info.PropertyType;

            if (!(propertyType.Equals(typeof(bool)) || propertyType.Equals(typeof(bool?))))
            {
                throw new InvalidOperationException("ItemCheckedPath defined source property is not of the appropriate type.");
            }

            if (!info.CanRead)
            {
                throw new InvalidOperationException("ItemCheckedPath defined source property is not readable.");
            }
        }
    }
}
