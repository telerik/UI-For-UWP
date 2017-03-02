using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CategoricalSeries : ExamplePageBase
    {
        public CategoricalSeries()
        {
            this.InitializeComponent();
        }
    }

    public class ViewModelCategorical
    {

        public ObservableCollection<CustomPointCategorical> Source
        {
            get
            {
                ObservableCollection<CustomPointCategorical> collection = new ObservableCollection<CustomPointCategorical>();
                collection.Add(new CustomPointCategorical { Category = "Apples", Value = 5 });
                collection.Add(new CustomPointCategorical { Category = "Oranges", Value = 9 });
                collection.Add(new CustomPointCategorical { Category = "Pineapples", Value = 8 });
                return collection;
            }
        }

        public class CustomPointCategorical
        {
            public string Category { get; set; }
            public double Value { get; set; }
        }
    }
}
