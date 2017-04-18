using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Charting;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Chart
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LabelStrategy : ExamplePageBase
    {
        public LabelStrategy()
        {
            this.InitializeComponent();
        }

    }

    public class CenterOutsideLabelStrategy : ChartSeriesLabelStrategy
    {
        public override LabelStrategyOptions Options
        {
            get
            {
                return LabelStrategyOptions.Arrange;
            }
        }

        public override RadRect GetLabelLayoutSlot(DataPoint point, FrameworkElement visual, int labelIndex)
        {
            visual.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            double x = point.LayoutSlot.X + ((point.LayoutSlot.Width - visual.ActualWidth) / 2);
            double y = point.LayoutSlot.Y - 20;

            return new RadRect(x, y, visual.ActualWidth, visual.ActualHeight);
        }
    }

    public class CenterInsideLabelStrategy : ChartSeriesLabelStrategy
    {
        public override LabelStrategyOptions Options
        {
            get
            {
                return LabelStrategyOptions.Arrange;
            }
        }

        public override RadRect GetLabelLayoutSlot(DataPoint point, FrameworkElement visual, int labelIndex)
        {
            visual.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            double x = point.LayoutSlot.X + ((point.LayoutSlot.Width - visual.ActualWidth) / 2);
            double y = point.LayoutSlot.Y + ((point.LayoutSlot.Height - visual.ActualHeight) / 2);

            return new RadRect(x, y, visual.ActualWidth, visual.ActualHeight);
        }
    }

    public class ChartDataLabelStrategy
    {
        public string Category { get; set; }

        public double Value { get; set; }

        public double Value2 { get; set; }

        public double CalculatedStackedSum
        {
            get
            {
                // you will use this property to visualize the stacked sum outside the bars
                return this.Value + this.Value2;
            }
        }
    }

    public class ViewModelLabelStrategy : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private List<ChartDataLabelStrategy> data;

        public ViewModelLabelStrategy()
        {
            this.Data = this.GetData();
        }

        public List<ChartDataLabelStrategy> Data
        {
            get
            {
                return this.data;
            }
            set
            {
                if (this.data != value)
                {
                    this.data = value;
                    this.OnPropertyChanged("Data");
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private List<ChartDataLabelStrategy> GetData()
        {
            List<ChartDataLabelStrategy> data = new List<ChartDataLabelStrategy>();
            data.Add(new ChartDataLabelStrategy() { Category = "A", Value = 10, Value2 = 20 });
            data.Add(new ChartDataLabelStrategy() { Category = "B", Value = 5, Value2 = 5 });
            data.Add(new ChartDataLabelStrategy() { Category = "C", Value = 15, Value2 = 5 });
            data.Add(new ChartDataLabelStrategy() { Category = "D", Value = 20, Value2 = 10 });
            data.Add(new ChartDataLabelStrategy() { Category = "E", Value = 25, Value2 = 15 });
            data.Add(new ChartDataLabelStrategy() { Category = "F", Value = 30, Value2 = 10 });

            return data;
        }
    }
}
