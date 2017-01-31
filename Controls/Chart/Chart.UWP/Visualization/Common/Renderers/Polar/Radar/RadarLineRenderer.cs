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
                // TODO: Actually Radar points can be sorted by category but do we need this? (cartesian categorical series do not support this right now as well).
                throw new NotSupportedException("Cannot sort points for Radar series.");
            }
        }
    }
}
