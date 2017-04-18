using System.Collections.Generic;
using Telerik.Data.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Listview
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Grouping : ExamplePageBase
    {
        private static string[] Surnames = new string[] { "Smith", "Williams", "Taylor", "Wilson", "Johnson", "White", "Walker" };
        private static string[] Names = new string[] { "James", "David", "Mary", "Susan", "Sarah", "Jason", "Robert" };


        public Grouping()
        {
            this.InitializeComponent();
            List<Item> list = new List<Item>();
            for (int i = 0; i < 100; i++)
            {
                list.Add(new Item { GroupId = i, LastName = Surnames[i % 7], FirstName = Names[(i / 7) % 7] });
            }
            this.DataContext = list;
        }

        public class Item
        {
            public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

            public string FirstName { get; set; }

            public string LastName { get; set; }
            public double GroupId { get; set; }
        }

        public class ListViewGroupKey : IKeyLookup
        {
            public object GetKey(object instance)
            {
                return string.Format("Group {0}", (instance as Item).GroupId % 2);
            }
        }

        private void GroupClick(object sender, RoutedEventArgs e)
        {
            this.ListView.GroupDescriptors.Clear();

            var descriptor = new DelegateGroupDescriptor() { KeyLookup = new ListViewGroupKey(), SortOrder = SortOrder.Descending };
            this.ListView.GroupDescriptors.Add(descriptor);
        }
    }


}
