using System;
using System.Collections.ObjectModel;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Controls;

namespace SDKExamples.UWP.DataGrid
{
    public sealed partial class ColumnDataOperationsModeExample : ExamplePageBase
    {
        public ColumnDataOperationsModeExample()
        {
            this.InitializeComponent();
            ObservableCollection<Item> source = new ObservableCollection<Item>();
            for (int i = 0; i < 10; i++)
            {
                source.Add(new Item() { Age = i * 10, Name = i.ToString() });
            }

            this.Combo.ItemsSource = Enum.GetValues(typeof(ColumnDataOperationsMode));
            this.Combo.SelectedItem = ColumnDataOperationsMode.Inline;
            this.DataContext = source;
        }

        public class Item
        {
            public string Name { get; set; }

            public double Age { get; set; }
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mode = (ColumnDataOperationsMode)(sender as ComboBox).SelectedItem;
            this.grid.ColumnDataOperationsMode = mode;
        }
    }
}
