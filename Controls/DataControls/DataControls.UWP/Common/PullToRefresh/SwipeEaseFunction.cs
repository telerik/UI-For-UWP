using System;

namespace Telerik.UI.Xaml.Controls.Data.Common
{
    /// <summary>
    /// Represent Parabola function to accelerate the original offset up. The ease used is EasyInOut :
    ///                       ____
    ///               L |    /
    ///                 |   /
    ///                 |  /
    ///                 | /
    ///                 |/_________
    ///                     offset   D
    /// In addition it translates the maxSwipeLength to stretchLimit (L) proportionally as MaxScrollStretch is the max stretch that scrollviewer can allow.
    /// The function is calculated as following:
    ///   - swipeTheshold / 2 >= acceleratedOffset --> 2 * swipeTheshold/maxSwipeLength^2 * offset^2
    ///   - acceleratedOffset > swipeTheshold / 2 and  swipeTheshold > swipeTheshold --> swipeTheshold - 2 * swipeTheshold /maxSwipeLength^2 * (maxSwipeLength - offset) ^ 2.
    /// </summary>
    internal class SwipeEaseFunction
    {
        private const double MaxScrollStretch = 85;
        private static double stretchLimit = MaxScrollStretch * 0.95;
        private double maxSwipeLength;
        private double accelerateCoef;
        private double accelerateCoef2;

        private double swipeTheshold;

        public SwipeEaseFunction(double swipeTheshold)
        {
            this.swipeTheshold = swipeTheshold;

            var minStretch = stretchLimit;
            this.maxSwipeLength = Math.Min(Math.Sqrt(this.swipeTheshold) + minStretch, stretchLimit);

            this.accelerateCoef = this.swipeTheshold / Math.Pow(this.maxSwipeLength, 2);

            this.accelerateCoef2 = 2 * this.swipeTheshold / this.maxSwipeLength;
        }

        public double GetAcceleratedOffset(double currentOffset, double previousAcceleratedOffset)
        {
            if (currentOffset < 0)
            {
                return previousAcceleratedOffset;
            }

            var offset = Math.Min(currentOffset, stretchLimit);

            var acceleratedOffset = offset;

            var previousOffset = previousAcceleratedOffset;

            if (previousOffset < this.swipeTheshold && offset < this.maxSwipeLength)
            {
                acceleratedOffset = -this.accelerateCoef * offset * offset + this.accelerateCoef2 * offset;
            }
            else
            {
                acceleratedOffset = this.swipeTheshold;
            }

            return acceleratedOffset;
        }
    }
}
