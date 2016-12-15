using System;
using Telerik.Data.Core;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents a descriptor that is used to filter items using an <see cref="IFilter"/> implementation.
    /// </summary>
    public class DelegateFilterDescriptor : FilterDescriptorBase
    {
        private DelegateFilterDescription filterDescription;
        private IFilter filter;

        /// <summary>
        /// Gets or sets the <see cref="IFilter"/> implementation used to check whether a data item passes the filter or not.
        /// </summary>
        public IFilter Filter
        {
            get
            {
                return this.filter;
            }
            set
            {
                if (this.filter == value)
                {
                    return;
                }

                this.filter = value;
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
                if (this.filter == null)
                {
                    throw new ArgumentNullException("Must have Filter provided.");
                }

                if (this.filterDescription == null)
                {
                    this.filterDescription = new DelegateFilterDescription(this);
                    this.filterDescription.Filter = this.filter.PassesFilter;
                }

                return this.filterDescription;
            }
        }

        internal override bool PassesFilter(object item)
        {
            if (this.filter == null)
            {
                return true;
            }

            return this.filter.PassesFilter(item);
        }

        // TBD: how to implement this for IFilter???
        internal override string SerializeToSQLiteWhereCondition()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Provides an entry point for inheritors to provide additional logic over the PropertyChanged routine.
        /// </summary>
        protected override void PropertyChangedOverride(string changedPropertyName)
        {
            if (this.filterDescription != null)
            {
                if (changedPropertyName == "Filter")
                {
                    this.filterDescription.Filter = this.filter.PassesFilter;
                }
            }

            base.PropertyChangedOverride(changedPropertyName);
        }
    }
}
