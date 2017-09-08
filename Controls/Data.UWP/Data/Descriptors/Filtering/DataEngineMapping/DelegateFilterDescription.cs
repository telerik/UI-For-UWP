using System;
using Telerik.Data.Core;

namespace Telerik.Data.Core
{
    internal class DelegateFilterDescription : PropertyFilterDescriptionBase
    {
        private Func<object, bool> filter;
        private Func<string> filterSqlRepresentation;

        public DelegateFilterDescription(object key)
        {
            this.Key = key;
        }

        public Func<object, bool> Filter
        {
            get
            {
                return this.filter;
            }
            set
            {
                if (this.filter != value)
                {
                    this.filter = value;
                    this.RaiseFilterChanged();
                }
            }
        }

        public Func<string> FilterSqlRepresentation
        {
            get { return this.filterSqlRepresentation; }
            set { this.filterSqlRepresentation = value; }
        }

        internal object Key
        {
            get;
            private set;
        }

        internal void RaiseFilterChanged()
        {
            this.OnPropertyChanged(nameof(this.Filter));
            this.OnSettingsChanged(new SettingsChangedEventArgs());
        }

        protected internal override object GetFilterItem(object fact)
        {
            return fact;
        }

        protected internal override bool PassesFilter(object value)
        {
            if (this.Filter != null)
            {
                return this.Filter(value);
            }

            return base.PassesFilter(value);
        }

        protected override void CloneOverride(Cloneable source)
        {
            var filterDescription = source as DelegateFilterDescription;
            this.Filter = filterDescription.Filter;
            this.FilterSqlRepresentation = filterDescription.FilterSqlRepresentation;
        }

        protected override Cloneable CreateInstanceCore()
        {
            return new DelegateFilterDescription(this.Key);
        }
    }
}
