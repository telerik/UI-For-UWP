using Telerik.Data.Core;

namespace Telerik.Data.Core
{
    internal class UserAggregateValue : AggregateValue
    {
        public IAggregateFunction Function
        {
            get;
            set;
        }

        protected override object GetValueOverride()
        {
            if (this.Function != null)
            {
                return this.Function.GetValue();
            }

            return null;
        }

        protected override void AccumulateOverride(object value)
        {
            if (this.Function != null)
            {
                this.Function.Accumulate(value);
            }
        }

        protected override void MergeOverride(AggregateValue childAggregate)
        {
            if (this.Function == null)
            {
                return;
            }

            var userAggregate = childAggregate as UserAggregateValue;
            if (userAggregate == null)
            {
                return;
            }

            this.Function.Merge(userAggregate.Function);
        }
    }
}
