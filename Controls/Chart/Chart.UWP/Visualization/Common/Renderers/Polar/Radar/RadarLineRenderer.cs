using System;
using System.Collections.Generic;
using Telerik.Charting;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal class RadarLineRenderer : PolarLineRendererBase
    {
        public RadarLineRenderer()
        {
            this.autoSortPoints = false;
        }

        internal override IComparer<DataPoint> Comparer
        {
            get
            {
                throw new NotSupportedException("Cannot sort points for Radar series.");
            }
        }
    }
}
