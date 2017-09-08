using System;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.Core
{
    /// <summary>
    /// Helper class, used for creating animations in code.
    /// </summary>
    internal class Easings
    {
        private static EasingFunctionBase circleIn;
        private static EasingFunctionBase circleOut;

        private static EasingFunctionBase slideDown1;
        private static EasingFunctionBase slideUp1;

        private static EasingFunctionBase quarticOut;
        private static EasingFunctionBase quiticOut;

        internal Easings(double x1, double y1, double x2, double y2)
        {
            this.Point1 = new Point(x1, y1);
            this.Point2 = new Point(x2, y2);
        }

        internal static EasingFunctionBase SlideDown1
        {
            get
            {
                return slideDown1 ?? (slideDown1 = new QuadraticEase());
            }
        }

        internal static EasingFunctionBase SlideUp1
        {
            get
            {
                return slideUp1 ?? (slideUp1 = new QuadraticEase());
            }
        }

        internal static Easings SlideDown2
        {
            get
            {
                return new Easings(0.264, 0, 0.228, 1);
            }
        }

        internal static Easings SlideUp2
        {
            get
            {
                return new Easings(0.224, 0, 0, 1);
            }
        }

        internal static Easings SlideDown3
        {
            get
            {
                return new Easings(0.02, 0.196, 0.362, 1);
            }
        }

        internal static Easings SlideUp3
        {
            get
            {
                return new Easings(0, 0.116, 0.431, 1);
            }
        }

        internal static EasingFunctionBase CircleOut
        {
            get
            {
                return circleOut ?? (circleOut = new CircleEase { EasingMode = EasingMode.EaseOut });
            }
        }

        internal static EasingFunctionBase CircleIn
        {
            get
            {
                return circleIn ?? (circleIn = new CircleEase { EasingMode = EasingMode.EaseIn });
            }
        }

        internal static EasingFunctionBase QuarticOut
        {
            get
            {
                return quarticOut ?? (quarticOut = new QuarticEase() { EasingMode = EasingMode.EaseOut });
            }
        }

        internal static EasingFunctionBase QuinticOut
        {
            get
            {
                return quiticOut ?? (quiticOut = new QuarticEase() { EasingMode = EasingMode.EaseOut });
            }
        }

        internal Point Point1 { get; set; }

        internal Point Point2 { get; set; }
    }
}