using System;

namespace Telerik.Data.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValueRangeAttribute : Attribute
    {
        public INumericalRange Range { get; set; } 

        public ValueRangeAttribute(double min, double max, double step)
        {
            this.Range = new NumericalRange(min, max,step);
        }
    }
}
