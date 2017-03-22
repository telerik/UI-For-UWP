using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PolarSeries : ExamplePageBase
    {
        public PolarSeries()
        {
            this.InitializeComponent();
        }
    }

    public class ViewModelPolarSeries
    {
        public ObservableCollection<FinancialData> Source
        {
            get
            {
                double a = 0.5;
                var b = (Math.PI / 180);
                ObservableCollection<FinancialData> collection = new ObservableCollection<FinancialData>();
                for (int i = 1; i < 360; i += 10)
                {
                    collection.Add(new FinancialData() { Angle = i, Value = Math.Abs((a * Math.Cos(20 * i * b)) )});
                }
                return collection;
            }
        }
    }

    public class FinancialData
    {
        public double Angle { get; set; }
        public double Value { get; set; }
    }
}
