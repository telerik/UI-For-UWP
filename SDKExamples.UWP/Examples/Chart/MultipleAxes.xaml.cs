using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MultipleAxes : ExamplePageBase
    {
        public MultipleAxes()
        {
            this.InitializeComponent();
        }

    }

    public class DataMultipleAxes
    {
        public string LineCategory { get; set; }
        public DateTime BarCategory { get; set; }
        public double LineValue { get; set; }
        public double BarValue { get; set; }
    }

    public class ViewModelMultipleAxes
    {

        public ObservableCollection<DataMultipleAxes> Source
        {
            get
            {
                ObservableCollection<DataMultipleAxes> collection = new ObservableCollection<DataMultipleAxes>();
                collection.Add(new DataMultipleAxes { LineCategory = "firstPoint", LineValue = 1, BarCategory = DateTime.Now.AddYears(1), BarValue = 10 });
                collection.Add(new DataMultipleAxes { LineCategory = "secondPoint", LineValue = 5, BarCategory = DateTime.Now.AddYears(2), BarValue = 11 });
                collection.Add(new DataMultipleAxes { LineCategory = "thirdPoint", LineValue = 2, BarCategory = DateTime.Now.AddYears(3), BarValue = 3 });
                return collection;
            }
        }
    }
}
