using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a descriptor that is used to group items by the value returned from a <see cref="IKeyLookup"/>.
    /// </summary>
    public class DelegateGroupDescriptor : GroupDescriptorBase
    {
        private IKeyLookup keyLookup;
        private DelegateGroupDescription groupDescription;
        private IFilter groupFilter;

        /// <summary>
        /// Gets or sets the <see cref="IKeyLookup"/> instance that is used to retrieve the group key for each data item.
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

        /// <summary>
        /// Gets or sets the <see cref="IFilter"/> implementation that may be used to filter the already generated groups.
        /// </summary>
        internal IFilter GroupFilter
        {
            get
            {
                return this.groupFilter;
            }
            set
            {
                this.groupFilter = value;
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

                if (this.groupDescription == null)
                {
                    this.groupDescription = new DelegateGroupDescription() { SortOrder = this.SortOrder };
                    this.groupDescription.MemberAccess = new DelegateMemberAccess() { Getter = this.keyLookup.GetKey };
                    this.groupDescription.GroupFilter = new DelegateGroupFilter() { FilterImpl = this.groupFilter };
                }

                return this.groupDescription;
            }
        }

        internal override bool Equals(DescriptionBase description)
        {
            var delegateDescription = description as DelegateGroupDescription;
            if (delegateDescription == null)
            {
                return false;
            }

            return delegateDescription.MemberAccess == this.groupDescription.MemberAccess && 
                delegateDescription.GroupFilter == this.groupDescription.GroupFilter;
        }

        /// <summary>
        /// Provides an entry point for inheritors to provide additional logic over the PropertyChanged routine.
        /// </summary>
        /// <param name="changedPropertyName"></param>
        protected override void PropertyChangedOverride(string changedPropertyName)
        {
            if (this.groupDescription != null)
            {
                if (changedPropertyName == "SortOrder")
                {
                    this.groupDescription.SortOrder = this.SortOrder;
                }
                else if (changedPropertyName == "KeyLookup" && this.keyLookup != null)
                {
                    this.groupDescription.MemberAccess = new DelegateMemberAccess() { Getter = this.keyLookup.GetKey };
                }
            }

            base.PropertyChangedOverride(changedPropertyName);
        }
    }
}
