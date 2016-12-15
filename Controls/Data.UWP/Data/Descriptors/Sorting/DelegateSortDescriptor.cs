using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents a descriptor that is used to sort items by the value returned by a delegate.
    /// </summary>
    public class DelegateSortDescriptor : SortDescriptorBase
    {
        private IKeyLookup keyLookup;
        private DelegateSortDescription sortDescription;

        /// <summary>
        /// Gets or sets the <see cref="IKeyLookup"/> instance that is used to retrieve the sort key for each data item.
        /// </summary>
        public IKeyLookup KeyLookup
        {
            get
            {
                return this.keyLookup;
            }
            set
            {
                this.keyLookup = value;
                this.OnPropertyChanged();
            }
        }

        internal override bool IsDelegate
        {
            get
            {
                return true;
            }
        }

        internal override DescriptionBase EngineDescription
        {
            get
            {
                if (this.keyLookup == null)
                {
                    throw new ArgumentNullException("Must have KeyExtractor provided.");
                }

                if (this.sortDescription == null)
                {
                    this.sortDescription = new DelegateSortDescription() { SortOrder = this.SortOrder, Comparer = this.Comparer };
                    this.sortDescription.MemberAccess = new DelegateMemberAccess() { Getter = this.keyLookup.GetKey };
                }

                return this.sortDescription;
            }
        }

        internal override bool Equals(DescriptionBase description)
        {
            var delegateDescription = description as DelegateSortDescription;
            if (delegateDescription == null)
            {
                return false;
            }

            return delegateDescription.MemberAccess == this.sortDescription.MemberAccess;
        }

        /// <summary>
        /// Properties the changed override.
        /// </summary>
        /// <param name="changedPropertyName">Name of the changed property.</param>
        protected override void PropertyChangedOverride(string changedPropertyName)
        {
            if (this.sortDescription != null)
            {
                if (changedPropertyName == "KeyLookup" && this.keyLookup != null)
                {
                    this.sortDescription.MemberAccess = new DelegateMemberAccess() { Getter = this.keyLookup.GetKey };
                }
                else if (changedPropertyName == "SortOrder")
                {
                    this.sortDescription.SortOrder = this.SortOrder;
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