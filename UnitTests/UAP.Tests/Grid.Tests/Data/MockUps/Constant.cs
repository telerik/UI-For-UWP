using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    internal class Constant : AggregateValue
    {
        private object value;

        public Constant(object value)
        {
            this.value = value;
        }

        protected override object GetValueOverride()
        {
            return this.value;
        }

        protected override void AccumulateOverride(object value)
        {
            throw new NotImplementedException();
        }

        protected override void MergeOverride(AggregateValue childAggregate)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return obj is Constant && Object.Equals(((Constant)obj).GetValue(), this.GetValue());
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
