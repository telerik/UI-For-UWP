using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SDKExamples.UWP.DataGrid
{
    public sealed partial class RowDetails : ExamplePageBase
    {
        private DataItem currentCheckedItem;

        public RowDetails()
        {
            this.InitializeComponent();

            this.DataGrid.ItemsSource = new List<DataItem>
            {
                new DataItem { Country = "United Kingdom", Capital = "London"},
                new DataItem { Country = "Germany", Capital = "Berlin"},
                new DataItem { Country = "Canada", Capital = "Otawa"},
                new DataItem { Country = "United States", Capital = "Washington"},
                new DataItem { Country = "Australia", Capital = "Canberra"}
            };

            foreach (DataItem item in this.DataGrid.ItemsSource as List<DataItem>)
            {
                item.Details = new ObservableCollection<Customer>();
                item.Details.Add(new Customer() { Name = "Karla Farver", Company = "Iceberg Systems", Email = "karla.farver@icebergsys.com" });
                item.Details.Add(new Customer() { Name = "Anton Donovan", Company = "Rushcorp", Email = "client@rushcorp.com" });
                item.Details.Add(new Customer() { Name = "Shellie Heron", Company = "Riverbite", Email = "shellie@riverbite.com" });
                item.Details.Add(new Customer() { Name = "Tom Haack", Company = "Aprico", Email = "tom.haack@aprico.com" });
            }
        }

        private void OnCheckBoxClick(object sender, RoutedEventArgs e)
        {
            var cb = (CheckBox)sender;
            var newCheckedItem = (DataItem)cb.DataContext;
            if (cb.IsChecked.HasValue && cb.IsChecked.Value)
            {
                this.DataGrid.ShowRowDetailsForItem(newCheckedItem);
            }
            else
            {
                this.DataGrid.HideRowDetailsForItem(newCheckedItem);
            }

            if (currentCheckedItem != null)
            {
                currentCheckedItem.HasRowDetails = false;
            }

            currentCheckedItem = newCheckedItem;
        }
    }

    public class DataItem
    {
        public string Country { get; set; }
        public string Capital { get; set; }
        public bool HasRowDetails { get; set; }
        public ObservableCollection<Customer> Details { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
    }
}
