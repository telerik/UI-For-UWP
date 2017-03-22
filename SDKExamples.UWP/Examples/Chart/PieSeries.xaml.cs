using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PieSeries : ExamplePageBase
    {
        public PieSeries()
        {
            this.InitializeComponent();
        }
    }

    public class ViewModel
    {
        public ObservableCollection<CustomPoint> Source
        {
            get
            {
                ObservableCollection<CustomPoint> collection = new ObservableCollection<CustomPoint>();
                collection.Add(new CustomPoint { Value = 6 });
                collection.Add(new CustomPoint { Value = 18 });
                collection.Add(new CustomPoint { Value = 3 });
                collection.Add(new CustomPoint { Value = 9 });
                return collection;
            }
        }
    }

    public class CustomPoint
    {
        public double Value { get; set; }
    }

}
