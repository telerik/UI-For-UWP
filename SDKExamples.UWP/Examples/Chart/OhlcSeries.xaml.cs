using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OhlcSeries : ExamplePageBase
    {
        public OhlcSeries()
        {
            this.InitializeComponent();
        }
    }

    public class ViewModelOhlc
    {
        public ObservableCollection<FinancialData> Source
        {
            get
            {
                ObservableCollection<FinancialData> collection = new ObservableCollection<FinancialData>();
                collection.Add(new FinancialData() { High = 10, Open = 5, Low = 2, Close = 8 });
                collection.Add(new FinancialData() { High = 15, Open = 7, Low = 3, Close = 5 });
                collection.Add(new FinancialData() { High = 20, Open = 15, Low = 10, Close = 19 });
                collection.Add(new FinancialData() { High = 7, Open = 2, Low = 1, Close = 5 });
                collection.Add(new FinancialData() { High = 25, Open = 15, Low = 10, Close = 12 });
                return collection;
            }
        }

        public class FinancialData
        {
            public double High { get; set; }
            public double Low { get; set; }
            public double Open { get; set; }
            public double Close { get; set; }
        }
    }
}
