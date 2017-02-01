using System;
using System.Diagnostics;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a <see cref="FilterDescriptorBase"/> that is associated with a particular property in the underlying ViewModel.
    /// </summary>
    public abstract class PropertyFilterDescriptor : FilterDescriptorBase, IPropertyDescriptor
    {
        private DelegateFilterDescription engineDescription;
        private string propertyName;
        private object value;

        /// <summary>
        /// Gets or sets the value used in the comparisons. This is the right operand of the comparison.
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that is used to retrieve the value to filter by.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
            set
            {
                if (this.propertyName == value)
                {
                    return;
                }

                this.propertyName = value;
                this.OnPropertyChanged();
            }
        }

        internal override DescriptionBase EngineDescription
        {
            get
            {
                if (this.engineDescription == null)
                {
                    this.engineDescription = new DelegateFilterDescription(this) { PropertyName = this.propertyName };
                    this.engineDescription.Filter = this.PassesFilter;
                    this.engineDescription.FilterSqlRepresentation = this.SerializeToSQLiteWhereCondition;
                }

                return this.engineDescription;
            }
        }

        internal IMemberAccess MemberAccess
        {
            get
            {
                return this.EngineDescription.MemberAccess;
            }
            set
            {
                this.EngineDescription.MemberAccess = value;
            }
        }

        internal override void AttachOverride()
        {
            base.AttachOverride();

            this.ResolveMemberAccess();
        }

        internal sealed override bool PassesFilter(object item)
        {
            if (this.MemberAccess == null)
            {
                Debug.Assert(false, "Must have member access at this point.");
                return false;
            }

            if (item == null)
            {
                return false;
            }

            var itemValue = this.MemberAccess.GetValue(item);
            return this.PassesFilterOverride(itemValue);
        }

        internal override string SerializeToSQLiteWhereCondition()
        {
            return this.SerializeToSQLiteWhereCondition();
        }

        /// <summary>
        /// Encapsulates the core filter logic exposed by the descriptor. Allows inheritors to provide their own custom filtering logic.
        /// </summary>
        /// <param name="itemValue">The property value, as defined by the <see cref="P:PropertyName"/> property.</param>
        /// <returns>True if the filter is passed and the associated item should be displayed, false otherwise.</returns>
        protected virtual bool PassesFilterOverride(object itemValue)
        {
            return false;
        }

        /// <summary>
        /// Provides an entry point for inheritors to provide additional logic over the PropertyChanged routine.
        /// </summary>
        /// <param name="changedPropertyName"></param>
        protected override void PropertyChangedOverride(string changedPropertyName)
        {
            if (this.Model != null && changedPropertyName == "PropertyName")
            {
                this.EngineDescription.PropertyName = this.propertyName;
                this.ResolveMemberAccess();
                this.UpdateAssociatedColumn();
            }

            if (this.engineDescription != null)
            {
                this.engineDescription.RaiseFilterChanged();
            }

            base.PropertyChangedOverride(changedPropertyName);
        }

        private void ResolveMemberAccess()
        {
            if (this.Model == null)
            {
                return;
            }

            if (this.Model.CurrentDataProvider == null || 
                this.Model.CurrentDataProvider.FieldDescriptions == null)
            {
                return;
            }

            var field = this.Model.CurrentDataProvider.FieldDescriptions.GetFieldDescriptionByMember(this.propertyName);
            if (field != null)
            {
                this.MemberAccess = field as IMemberAccess;
            }
        }
    }
}
