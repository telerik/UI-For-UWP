using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.Data.Core;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Listview
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Filtering : ExamplePageBase
    {
        ObservableCollection<Data> source = new ObservableCollection<Data>();

        public Filtering()
        {
            this.InitializeComponent();

            this.source.Add(new Data() { Name = "Ivaylo", Age = 26, IsMarried = false, Date = DateTime.Now });
            this.source.Add(new Data() { Name = "Ivo", Age = 25, IsMarried = true, Date = DateTime.Now.AddYears(1) });
            this.source.Add(new Data() { Name = "Vladislav", Age = 27, IsMarried = true, Date = DateTime.Now.AddYears(1) });
            this.source.Add(new Data() { Name = "Petar", Age = 24, IsMarried = false, Date = DateTime.Now.AddYears(2) });

            this.filterCombo.ItemsSource = new List<string>() { "None", "Name", "Age", "IsMarried", "Date", "Custom" };
            this.filterCombo.SelectedIndex = 0;
            this.DataContext = source;
        }


        public class Data
        {
            public string Name { get; set; }
            public double Age { get; set; }
            public bool IsMarried { get; set; }
            public DateTime Date { get; set; }
        }

        private void FilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedString = (sender as ComboBox).SelectedItem.ToString();
            this.listView.FilterDescriptors.Clear();
            switch (selectedString)
            {
                case "Name":
                    this.listView.FilterDescriptors.Add(new TextFilterDescriptor()
                    {
                        PropertyName = "Name",
                        Value = "Iv",
                        IsCaseSensitive = true,
                        Operator = TextOperator.Contains
                    });
                    break;
                case "Age":
                    this.listView.FilterDescriptors.Add(new NumericalFilterDescriptor()
                    {
                        PropertyName = "Age",
                        Value = 26,
                        Operator = NumericalOperator.IsLessThan
                    });
                    break;
                case "IsMarried":

                    this.listView.FilterDescriptors.Add(new BooleanFilterDescriptor()
                    {
                        PropertyName = "IsMarried",
                        Value = true
                    });
                    break;
                case "Date":
                    this.listView.FilterDescriptors.Add(new DateTimeFilterDescriptor()
                    {
                        PropertyName = "Date",
                        Value = DateTime.Now,
                        Part = DateTimePart.Date,
                        Operator = NumericalOperator.IsGreaterThan,
                    });
                    break;

                case "Custom":
                    this.listView.FilterDescriptors.Add(new DelegateFilterDescriptor()
                    {
                        Filter = new CustomFilter()
                    });
                    break;
            }
        }

        public class CustomFilter : IFilter
        {
            public bool PassesFilter(object item)
            {
                var data = item as Data;
                if (data.Age % 2 == 0 && data.Date.Year <= DateTime.Now.Year)
                {
                    return true;
                }

                return false;
            }
        }

    }
}
