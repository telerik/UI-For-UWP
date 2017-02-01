using System;

namespace Telerik.Data.Core.Aggregates
{
    internal class ConstantValueAggregate : AggregateValue
    {
        private object value;
        public ConstantValueAggregate(object value)
        {
            this.value = value;
        }

        protected override object GetValueOverride()
        {
            return this.value;
        }

        protected override void AccumulateOverride(object item)
        {
            throw new NotImplementedException();
        }

        protected override void MergeOverride(AggregateValue childAggregate)
        {
            throw new NotImplementedException();
        }
    }
}
