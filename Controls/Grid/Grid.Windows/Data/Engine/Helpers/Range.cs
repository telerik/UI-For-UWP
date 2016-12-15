// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Telerik.Data.Core
{
    internal class Range<T>
    {
        public Range(int lowerBound, int upperBound, T value)
        {
            this.LowerBound = lowerBound;
            this.UpperBound = upperBound;
            this.Value = value;
        }

        public int Count
        {
            get
            {
                return this.UpperBound - this.LowerBound + 1;
            }
        }

        public int LowerBound
        {
            get;
            set;
        }

        public int UpperBound
        {
            get;
            set;
        }

        public T Value
        {
            get;
            set;
        }

        public bool ContainsIndex(int index)
        {
            return (this.LowerBound <= index) && (this.UpperBound >= index);
        }

        public bool ContainsValue(object value)
        {
            if (this.Value == null)
            {
                return value == null;
            }
            else
            {
                return this.Value.Equals(value);
            }
        }

        public Range<T> Copy()
        {
            return new Range<T>(this.LowerBound, this.UpperBound, this.Value);
        }
    }
}