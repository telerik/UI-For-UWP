using System;
using Telerik.Data.Core;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents a descriptor that is used to group items by the value of a property in each data item.
    /// </summary>
    public class PropertyGroupDescriptor : GroupDescriptorBase, IPropertyDescriptor
    {
        private string propertyName;
        private PropertyGroupDescription groupDescription;

        /// <summary>
        /// Gets or sets the name of the property that is used to retrieve the key to group by.
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
                if (this.groupDescription == null)
                {
                    this.groupDescription = new PropertyGroupDescription() { PropertyName = this.propertyName, SortOrder = this.SortOrder };
                }

                return this.groupDescription;
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
            return this.propertyName;
        }

        internal override bool Equals(DescriptionBase description)
        {
            var propertyDescription = description as PropertyGroupDescription;
            if (propertyDescription == null)
            {
                return false;
            }

            return propertyDescription.PropertyName == this.propertyName;
        }

        /// <summary>
        /// Provides an entry point for inheritors to provide additional logic over the PropertyChanged routine.
        /// </summary>
        protected override void PropertyChangedOverride(string changedPropertyName = "")
        {
            if (this.groupDescription != null)
            {
                if (changedPropertyName == "SortOrder")
                {
                    this.groupDescription.SortOrder = this.SortOrder;
                }
                else if (changedPropertyName == "PropertyName")
                {
                    this.groupDescription.PropertyName = this.propertyName;
                    this.UpdateAssociatedPeer();
                }
            }

            base.PropertyChangedOverride(changedPropertyName);
        }
    }
}
