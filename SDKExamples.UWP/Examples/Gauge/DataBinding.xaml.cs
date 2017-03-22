using Telerik.Core;
using Windows.UI.Xaml.Controls;

namespace SDKExamples.UWP.Gauge
{
    public sealed partial class DataBinding : ExamplePageBase
    {
        public DataBinding()
        {
            this.InitializeComponent();
            this.DataContext = new Model();
        }

        public class Model : ViewModelBase
        {
            private double temperature;

            public Model()
            {
                this.Temperature = 35;
                this.Min = 20;
                this.Average = 30;
                this.Max = 50;
            }

            public double Temperature
            {
                get
                {
                    return this.temperature;
                }
                set
                {
                    if (this.temperature != value)
                    {
                        this.temperature = value;
                        this.OnPropertyChanged();
                    }
                }
            }

            public double Min { get; set; }

            public double Average { get; set; }

            public double Max { get; set; }
        }
    }
}
