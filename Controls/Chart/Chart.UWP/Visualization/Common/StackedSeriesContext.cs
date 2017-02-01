using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Stores information about stacked series during an UpdateUI pass.
    /// </summary>
    internal class StackedSeriesContext
    {
        public List<Point> PreviousStackedArea;

        public void Clear()
        {
            this.PreviousStackedArea = null;
        }
    }
}
