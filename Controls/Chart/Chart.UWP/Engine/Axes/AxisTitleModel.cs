using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal class AxisTitleModel : ContentNode
    {
        public AxisTitleModel()
        {
            this.TrackPropertyChanged = true;
        }

        internal override void UnloadCore()
        {
            this.desiredSize = RadSize.Empty;
            base.UnloadCore();
        }
    }
}
