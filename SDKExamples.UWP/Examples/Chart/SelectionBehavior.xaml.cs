using System;
using Telerik.Charting;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml.Controls;
using System.Linq;
using System.Collections.Generic;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SelectionBehavior : ExamplePageBase
    {
        public SelectionBehavior()
        {
            this.InitializeComponent();
        }

        private void DataPointSelectionChanged(object sender, EventArgs e)
        {
            if ((sender as ChartSelectionBehavior).SelectedPoints.Count<DataPoint>() > 0)
            {
                CategoricalDataPoint selectedPoint = (sender as ChartSelectionBehavior).SelectedPoints.First() as CategoricalDataPoint;
                textblock1.Text = string.Format("The selected point value is : {0}", selectedPoint.Value);
                textblock2.Text = string.Format("The selected point category is : {0}", selectedPoint.Category);
                textblock3.Text = string.Format("View model property value: {0}", (selectedPoint.DataItem as CustomPointSelection).CustomProperty);
            }
            else
            {
                textblock1.Text = "No point selected";
                textblock2.Text = "No point selected";
                textblock3.Text = "No point selected";
            }
        }
    }

    public class CustomPointSelection
    {
        public double Value { get; set; }
        public string Category { get; set; }
        public string CustomProperty { get; set; }
    }

    public class ViewModelSelection
    {
        public ViewModelSelection()
        {
            this.Source = new List<CustomPointSelection>()
            {
                new CustomPointSelection{ Category = "Apples", Value = 10, CustomProperty =  "First point" },
                new CustomPointSelection{ Category = "Oranges", Value = 32, CustomProperty = "Second point"},
                new CustomPointSelection{ Category = "Pears", Value = 15, CustomProperty =   "Third point"},
            };
        }

        public List<CustomPointSelection> Source { get; set; }
    }
}
