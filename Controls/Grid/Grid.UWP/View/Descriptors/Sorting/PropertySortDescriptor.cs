using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a descriptor that is used to sort items by the value of a property in each data item.
    /// </summary>
    public class PropertySortDescriptor : SortDescriptorBase, IPropertyDescriptor
    {
        private string propertyName;
        private PropertySortDescription sortDescription;

        /// <summary>
        /// Gets or sets the name of the property that is used to retrieve the key to sort by.
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
                if (this.sortDescription == null)
                {
                    this.sortDescription = new PropertySortDescription() { PropertyName = this.propertyName, SortOrder = this.SortOrder };
                }

                return this.sortDescription;
            }
        }

        internal override bool Equals(DescriptionBase description)
        {
            var propertyDescription = description as PropertySortDescription;
            if (propertyDescription == null)
            {
                return false;
            }

            return propertyDescription.PropertyName == this.PropertyName;
        }

        /// <summary>
        /// Provides an entry point for inheritors to provide additional logic over the PropertyChanged routine.
        /// </summary>
        /// <param name="changedPropertyName"></param>
        protected override void PropertyChangedOverride(string changedPropertyName)
        {
            if (this.sortDescription != null)
            {
                if (changedPropertyName == "SortOrder")
                {
                    this.sortDescription.SortOrder = this.SortOrder;
                }
                else if (changedPropertyName == "PropertyName")
                {
                    this.sortDescription.PropertyName = this.propertyName;
                    this.UpdateAssociatedColumn();
                }
            }

            base.PropertyChangedOverride(changedPropertyName);
        }
    }
}
