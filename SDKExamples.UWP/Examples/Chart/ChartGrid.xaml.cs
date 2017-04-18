using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChartGrid : ExamplePageBase
    {
        public ChartGrid()
        {
            this.InitializeComponent();
        }
    }

    public class CustomPointChartGrid
    {
        public string Category { get; set; }
        public double Value { get; set; }
    }

    public class ViewModelChartGrid
    {
        public ViewModelChartGrid()
        {
            this.Source = new List<CustomPointChartGrid>()
            {
                new CustomPointChartGrid{ Category = "Apples", Value = 10 },
                new CustomPointChartGrid{ Category = "Oranges", Value = 32 },
                new CustomPointChartGrid{ Category = "Pears", Value = 15 },
            };
        }
        public List<CustomPointChartGrid> Source { get; set; }
    }
}
