using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomPalette : ExamplePageBase
    {
        public CustomPalette()
        {
            this.InitializeComponent();
        }
    }

    public class CustomPalettes : ChartPalette
    {
        public CustomPalettes()
        {
            // fill
            this.FillEntries.Brushes.Add(new SolidColorBrush(Colors.Bisque));
            this.FillEntries.Brushes.Add(new SolidColorBrush(Colors.Gray));
            this.FillEntries.Brushes.Add(new SolidColorBrush(Colors.DarkGreen));
            // stroke
            this.StrokeEntries.Brushes.Add(new SolidColorBrush(Colors.White));
            this.StrokeEntries.Brushes.Add(new SolidColorBrush(Colors.Gray));
            this.StrokeEntries.Brushes.Add(new SolidColorBrush(Colors.DarkGreen));
        }
    }

    public class CustomPointPalette
    {
        public double Value { get; set; }
        public string Category { get; set; }
    }

    public class ViewModelPalette
    {
        public ViewModelPalette()
        {
            this.Source = new List<CustomPointPalette>()
                                                    {
                                                        new CustomPointPalette{ Category = "first", Value = 1 },
                                                        new CustomPointPalette{ Category = "second", Value = 2 },
                                                        new CustomPointPalette{ Category = "third", Value = 3 },

                                                    };
        }
        public List<CustomPointPalette> Source { get; set; }
    }
}
