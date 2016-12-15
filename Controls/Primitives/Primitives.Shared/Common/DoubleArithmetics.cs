using System;
using Telerik.Core;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    internal class DoubleArithmetics
    {
        private int precision = 100;

        public DoubleArithmetics(int precision)
        {
            this.precision = precision;
        }

        public double Floor(double value)
        {
            return Math.Floor(value * this.precision) / this.precision;
        }

        public double Ceiling(double value)
        {
            return Math.Ceiling(value * this.precision) / this.precision;
        }

        public RadSize Ceiling(RadSize value)
        {
            return new RadSize(this.Ceiling(value.Width), this.Ceiling(value.Height));
        }

        public bool AreClose(RadSize size1, RadSize size2)
        {
            return this.AreClose(size1.Width, size2.Width) && this.AreClose(size1.Height, size2.Height);
        }

        public bool AreClose(Size size1, Size size2)
        {
            return this.AreClose(size1.Width, size2.Width) && this.AreClose(size1.Height, size2.Height);
        }

        public bool IsZero(double value)
        {
            return (long)Math.Ceiling(value * this.precision) == 0;
        }

        public bool AreClose(double value1, double value2)
        {
            return (long)Math.Ceiling(value1 * this.precision) == (long)Math.Ceiling(value2 * this.precision);
        }

        public bool IsLessThan(double value1, double value2)
        {
            return (long)Math.Ceiling(value1 * this.precision) < (long)Math.Ceiling(value2 * this.precision);
        }

        public bool IsLessThanOrEqual(double value1, double value2)
        {
            return (long)Math.Ceiling(value1 * this.precision) <= (long)Math.Ceiling(value2 * this.precision);
        }

        public bool IsGreaterThan(double value1, double value2)
        {
            return (long)Math.Ceiling(value1 * this.precision) > (long)Math.Ceiling(value2 * this.precision);
        }

        public bool IsGreaterThanOrEqual(double value1, double value2)
        {
            return (long)Math.Ceiling(value1 * this.precision) >= (long)Math.Ceiling(value2 * this.precision);
        }
    }
}
