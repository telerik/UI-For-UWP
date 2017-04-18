using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SDKExamples.UWP.DataGrid
{
    public sealed partial class GroupPanelPosition : ExamplePageBase
    {
        private static Random rand = new Random(0);

        public GroupPanelPosition()
        {
            this.InitializeComponent();
            this.DataContext = new ObservableCollection<Item>(Enumerable.Range(0, 10).Select(i => new Item { Age = rand.Next(15, 30), Name = "Customer " + i }));
        }

        public class Item
        {
            public string Name { get; set; }
            public double Age { get; set; }
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.grid.GroupPanelPosition == Telerik.UI.Xaml.Controls.Grid.GroupPanelPosition.Left)
            {
                this.grid.GroupPanelPosition = Telerik.UI.Xaml.Controls.Grid.GroupPanelPosition.Bottom;
            }
            else
            {
                this.grid.GroupPanelPosition = Telerik.UI.Xaml.Controls.Grid.GroupPanelPosition.Left;
            }
        }
    }
}
