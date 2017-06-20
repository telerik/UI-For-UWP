using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PanAndZoom : ExamplePageBase
    {
        public PanAndZoom()
        {
            this.InitializeComponent();
        }

        private void RadCartesianChart_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var chart = sender as RadCartesianChart;
            if (chart != null)
            {
                var containerVisualFactory = chart.ContainerVisualsFactory as AnimationContainerVisualsFactory;
                if (containerVisualFactory != null)
                {
                    containerVisualFactory.TriggerOrderedVisualsAnimation(35, 450);
                }
            }
        }
    }

    public class CustomPointPanAndZoom
    {
        public double Value { get; set; }
        public string Category { get; set; }
    }

    public class ViewModelPanAndZoom
    {
        public ViewModelPanAndZoom()
        {
            List<CustomPointPanAndZoom> points = new List<CustomPointPanAndZoom>();

            for (int i = 0; i < 100; i++)
            {
                points.Add(new CustomPointPanAndZoom { Category = "cat" + i, Value = i });
            }

            this.Source = points;
        }
        public List<CustomPointPanAndZoom> Source { get; set; }
    }
}
