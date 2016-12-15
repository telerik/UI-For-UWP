using System;

namespace Telerik.Data.Core.Aggregates
{
    internal class UnsupportedTypeAggregate : AggregateValue
    {
        public UnsupportedTypeAggregate()
        {
            this.RaiseError(); // TODO: Better error support. Raise an UnsupportedError of type AggregateError. Once with error do not Accumulate or Merge further...
        }

        protected override object GetValueOverride()
        {
            throw new NotImplementedException();
        }

        protected override void AccumulateOverride(object value)
        {
            throw new NotImplementedException();
        }

        protected override void MergeOverride(AggregateValue childAggregate)
        {
            throw new NotImplementedException();
        }
    }
}
