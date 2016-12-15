using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Core;
using Telerik.Data.Core;

namespace Telerik.Data.Core
{
    public abstract class DataDescriptorBase : ViewModelBase, IDataDescriptor
    {
        //private WeakReference<DataGridColumn> associatedColumn;

        //internal GridModel Model
        //{
        //    get;
        //    set;
        //}

        internal virtual DataChangeFlags UpdateFlags
        {
            get
            {
                return DataChangeFlags.None;
            }
        }

        internal abstract DescriptionBase EngineDescription
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the current instance is DelegateDataDescriptor - that is to have a user-specified member access.
        /// </summary>
        internal virtual bool IsDelegate
        {
            get
            {
                return false;
            }
        }

        ///// <summary>
        ///// Gets the <see cref="DataGridColumn"/> column associated with this descriptor.
        ///// This property is updated when the descriptor is added to the corresponding Descriptors collection of the owning <see cref="RadDataGrid"/>.
        ///// </summary>
        //internal DataGridColumn AssociatedColumn
        //{
        //    get
        //    {
        //        if (this.associatedColumn == null)
        //        {
        //            return null;
        //        }

        //        DataGridColumn column;
        //        if (this.associatedColumn.TryGetTarget(out column))
        //        {
        //            return column;
        //        }

        //        return null;
        //    }
        //    set
        //    {
        //        this.associatedColumn = new WeakReference<DataGridColumn>(value);
        //    }
        //}

        internal abstract bool Equals(DescriptionBase description);

        //internal virtual void Attach(GridModel model)
        //{
        //    //this.Model = model;
        //    //this.UpdateAssociatedColumn();
        //    this.AttachOverride();
        //}

        //internal void Detach()
        //{
        //    this.Model = null;
        //    this.DetachAssociatedColumn();
        //}

        //internal virtual bool IsAssociatedColumn(DataGridColumn column)
        //{
        //    var propertyDescriptor = this as IPropertyDescriptor;
        //    if (propertyDescriptor == null)
        //    {
        //        return false;
        //    }

        //    if (column != null)
        //    {
        //        return column.IsAssociatedWithDescriptor(propertyDescriptor);
        //    }

        //    return false;
        //}

        //internal virtual void AttachOverride()
        //{
        //}

        ///// <summary>
        ///// Resolves the column that is considered as "Associated" with the current instance.
        ///// For example such an association may occur between a <see cref="PropertySortDescriptor"/> and a <see cref="DataGridTypedColumn"/> instance.
        ///// </summary>
        //internal void UpdateAssociatedColumn(DataGridColumn column = null)
        //{
        //    this.DetachAssociatedColumn();

        //    if (column == null)
        //    {
        //        column = this.FindAssociatedColumn();
        //    }

        //    this.AssociatedColumn = column;

        //    if (column != null)
        //    {
        //        column.OnDescriptorAssociated(this);
        //    }
        //}

        ///// <summary>
        ///// Provides an entry point for inheritors to provide additional logic over the PropertyChanged routine.
        ///// </summary>
        ///// <param name="changedPropertyName"></param>
        //protected override void PropertyChangedOverride(string changedPropertyName)
        //{
        //    base.PropertyChangedOverride(changedPropertyName);

        //    var column = this.AssociatedColumn;
        //    if (column != null)
        //    {
        //        column.OnAssociatedDescriptorPropertyChanged(this, changedPropertyName);
        //    }
        //}

        //private void DetachAssociatedColumn()
        //{
        //    var column = this.AssociatedColumn;
        //    if (column != null)
        //    {
        //        column.OnAssociatedDescriptorRemoved(this);
        //    }
        //}

        //private DataGridColumn FindAssociatedColumn()
        //{
        //    if (this.Model == null)
        //    {
        //        return null;
        //    }

        //    foreach (var column in this.Model.columns)
        //    {
        //        if (this.IsAssociatedColumn(column))
        //        {
        //            return column;
        //        }
        //    }

        //    return null;
        //}

        //internal DescriptionBase EngineDescription
        //{
        //    get { return this.EngineDescription; }
        //}

        internal void DetachFromHost()
        {
            throw new NotImplementedException();
        }

        internal void AttachToHost(IDataDescriptorsHost host)
        {
            throw new NotImplementedException();
        }

    }
}
