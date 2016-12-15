using System;
using Telerik.Data.Core;

namespace Telerik.Data.Core
{
    internal class DelegateGroupFilter : SingleGroupFilter
    {
        public DelegateGroupFilter()
        {
        }

        public IFilter FilterImpl
        {
            get;
            set;
        }

        internal override bool Filter(IGroup group, IAggregateResultProvider results, DataAxis axis)
        {
            if (this.FilterImpl != null)
            {
                return this.FilterImpl.PassesFilter(group);
            }

            return true;
        }

        protected override Cloneable CreateInstanceCore()
        {
            return new DelegateGroupFilter();
        }

        protected override void CloneCore(Cloneable source)
        {
            var delegateGroupFilter = source as DelegateGroupFilter;
            this.FilterImpl = delegateGroupFilter.FilterImpl;
        }
    }
}
