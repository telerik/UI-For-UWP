using System;
using Telerik.Core;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal struct DoughnutSegmentData
    {
        public RadPoint Center;
        public double StartAngle;
        public double SweepAngle;
        public double Radius1;
        public double Radius2;
        public SweepDirection SweepDirection;
    }
}