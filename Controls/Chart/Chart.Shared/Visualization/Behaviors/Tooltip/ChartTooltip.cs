using System;
using System.ComponentModel;
using System.Windows;
using Telerik.Charting;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// This class represents the tool tips for RadChart.
    /// </summary>
    public class ChartTooltip : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the ChartTooltip class.
        /// </summary>
        public ChartTooltip()
        {
            this.DefaultStyleKey = typeof(ChartTooltip);
        }
    }
}
