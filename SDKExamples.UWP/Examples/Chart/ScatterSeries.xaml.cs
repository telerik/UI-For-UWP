using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScatterSeries : ExamplePageBase
    {
        public ScatterSeries()
        {
            this.InitializeComponent();
        }
    }

    public class ViewModelScatterSeries
    {

        public ObservableCollection<CustomPointScatter> Source
        {
            get
            {
                ObservableCollection<CustomPointScatter> collection = new ObservableCollection<CustomPointScatter>();
                collection.Add(new CustomPointScatter { XValue = 2, YValue = 3 });
                collection.Add(new CustomPointScatter { XValue = 5, YValue = 6 });
                collection.Add(new CustomPointScatter { XValue = 8, YValue = 3 });
                collection.Add(new CustomPointScatter { XValue = 10, YValue = 5 });
                collection.Add(new CustomPointScatter { XValue = 14, YValue = 1 });
                return collection;
            }
        }

        public class CustomPointScatter
        {
            public double XValue { get; set; }
            public double YValue { get; set; }
        }
    }
}
