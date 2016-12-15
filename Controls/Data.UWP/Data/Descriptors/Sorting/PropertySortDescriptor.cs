using System;

namespace Telerik.Data.Core
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
                    this.sortDescription = new PropertySortDescription() { PropertyName = this.propertyName, SortOrder = this.SortOrder, Comparer = this.Comparer };
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
                    this.UpdateAssociatedPeer();
                }
                else if (changedPropertyName == "Comparer")
                {
                    this.sortDescription.Comparer = this.Comparer;
                }
            }

            base.PropertyChangedOverride(changedPropertyName);
        }
    }
}