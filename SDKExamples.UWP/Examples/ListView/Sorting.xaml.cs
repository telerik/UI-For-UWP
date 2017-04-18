using System;
using System.Collections.Generic;
using Telerik.Data.Core;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Listview
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Sorting : ExamplePageBase

    {
        public Sorting()
        {
            this.InitializeComponent();

 
            List<Item> list = new List<Item>();
            list.Add(new Item() { FirstName = "Ivaylo", LastName = "Gergov" });
            list.Add(new Item() { FirstName = "Ivo", LastName = "Petko" });
            list.Add(new Item() { FirstName = "Yanaki", LastName = "Dolapchiev" });
            list.Add(new Item() { FirstName = "Krisko", LastName = "Hristov" });

            this.DataContext = list;
        }

        public class Item
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class CustomLookup : IKeyLookup
        {
            public object GetKey(object instance)
            {
                var item = instance as Item;

                return item.FirstName.Length + item.LastName.Length;
            }
        }

        private void SortButtonClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DelegateSortDescriptor descriptor = new DelegateSortDescriptor();
            descriptor.KeyLookup = new CustomLookup();
            descriptor.SortOrder = SortOrder.Ascending;
            this.ListView.SortDescriptors.Clear();
            this.ListView.SortDescriptors.Add(descriptor);
        }
    }
}
