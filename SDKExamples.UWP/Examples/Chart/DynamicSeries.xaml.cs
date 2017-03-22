using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DynamicSeries : ExamplePageBase
    {
        public DynamicSeries()
        {
            this.InitializeComponent();

            this.provider.Source = GenerateCollection();
        }

        public List<CustomPointDynamic> GenerateData(int i)
        {
            List<CustomPointDynamic> data = new List<CustomPointDynamic>();
            data.Add(new CustomPointDynamic { Category = "Apple", Value = 4 + i });
            data.Add(new CustomPointDynamic { Category = "Orange", Value = 15 - i });
            data.Add(new CustomPointDynamic { Category = "Lemon", Value = 20 + i * i });
            return data;
        }

        public List<ViewModelDynamic> GenerateCollection()
        {
            List<ViewModelDynamic> collection = new List<ViewModelDynamic>();
            for (int i = 0; i < 5; i++)
            {
                ViewModelDynamic vm = new ViewModelDynamic();
                vm.GetData = GenerateData(i);
                collection.Add(vm);
            }
            return collection;
        }

        public class CustomPointDynamic
        {
            public double Value { get; set; }
            public string Category { get; set; }
        }

        public class ViewModelDynamic
        {
            public List<CustomPointDynamic> GetData { get; set; }
        }
    }
}
