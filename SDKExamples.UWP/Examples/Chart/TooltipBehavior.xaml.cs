using System;
using System.Linq;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TooltipBehavior : ExamplePageBase
    {
        public TooltipBehavior()
        {
            this.InitializeComponent();
        }

        private void chart_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var chart = sender as RadCartesianChart;
            if (chart != null)
            {
                var containerVisualFactory = chart.ContainerVisualsFactory as AnimationContainerVisualsFactory;
                if (containerVisualFactory != null)
                {
                    containerVisualFactory.TriggerOrderedVisualsAnimation(50, 500);
                }
            }
        }
    }

    public class CustomPointTooltip
    {
        public string Category { get; set; }
        public double Value { get; set; }
        public double SecondValue { get; set; }
        public string Country { get; set; }
        public string SecondCountry { get; set; }
    }

    public class ViewModelTooltip
    {
        public ViewModelTooltip()
        {
            this.Source = this.GetData();
        }
        public List<CustomPointTooltip> Source
        {
            get;
            set;
        }

        private List<CustomPointTooltip> GetData()
        {
            List<CustomPointTooltip> data = new List<CustomPointTooltip>();
            data.Add(new CustomPointTooltip() { Category = "Apples", Value = 10, SecondValue = 5, Country = "Bulgaria", SecondCountry = "Italy" });
            data.Add(new CustomPointTooltip() { Category = "Oranges", Value = 12, SecondValue = 9, Country = "Brazil", SecondCountry = "USA" });
            data.Add(new CustomPointTooltip() { Category = "Pineaples", Value = 8, SecondValue = 13, Country = "Philippines", SecondCountry = "Brazil" });
            return data;
        }
    }

    public class CustomConverterBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BarSeries series = (value as DataPointInfo).Series as BarSeries;
            RadCartesianChart chart = series.Chart as RadCartesianChart;
            var dataPoint = (value as DataPointInfo).DataPoint;
            Border border = series.GetDataPointVisual(dataPoint) as Border;
            if (border != null)
            {
                return border.Background;
            }

            return new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.Format("{0} is {1}", parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
