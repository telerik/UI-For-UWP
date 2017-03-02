using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RadarSeries : ExamplePageBase
    {
        public RadarSeries()
        {
            this.InitializeComponent();
        }
    }

    public class ViewModelRadarSeries
    {
        public ObservableCollection<CustomPointRadar> Source
        {
            get
            {
                ObservableCollection<CustomPointRadar> collection = new ObservableCollection<CustomPointRadar>();
                for (double i = 1; i < 30; i += 1)
                {
                    collection.Add(new CustomPointRadar() { Category = i, Value = ((0.7) * Math.Cos(20 * i)) });
                }
                return collection;
            }
        }

        public class CustomPointRadar
        {
            public double Value { get; set; }
            public double Category { get; set; }
        }
    }
}
