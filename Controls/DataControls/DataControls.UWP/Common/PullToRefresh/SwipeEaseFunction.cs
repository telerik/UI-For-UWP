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
    /// In addition it translates the maxSwipeLength to stretchLimit (L) proportionally as MaxScrollStretch is the max stretch that ScrollViewer can allow.
    /// The function is calculated as following:
    ///   - swipeThreshold / 2 >= acceleratedOffset --> 2 * swipeThreshold/maxSwipeLength^2 * offset^2
    ///   - acceleratedOffset > swipeThreshold / 2 and  swipeThreshold > swipeThreshold --> swipeThreshold - 2 * swipeThreshold /maxSwipeLength^2 * (maxSwipeLength - offset) ^ 2.
    /// </summary>
    internal class SwipeEaseFunction
    {
        private const double MaxScrollStretch = 85;
        private static double stretchLimit = MaxScrollStretch * 0.95;
        private double maxSwipeLength;
        private double accelerateCoef;
        private double accelerateCoef2;

        private double swipeThreshold;

        public SwipeEaseFunction(double swipeThreshold)
        {
            this.swipeThreshold = swipeThreshold;

            var minStretch = stretchLimit;
            this.maxSwipeLength = Math.Min(Math.Sqrt(this.swipeThreshold) + minStretch, stretchLimit);

            this.accelerateCoef = this.swipeThreshold / Math.Pow(this.maxSwipeLength, 2);

            this.accelerateCoef2 = 2 * this.swipeThreshold / this.maxSwipeLength;
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

            if (previousOffset < this.swipeThreshold && offset < this.maxSwipeLength)
            {
                acceleratedOffset = -this.accelerateCoef * offset * offset + this.accelerateCoef2 * offset;
            }
            else
            {
                acceleratedOffset = this.swipeThreshold;
            }

            return acceleratedOffset;
        }
    }
}
