using System.Collections.ObjectModel;
using System.Linq;
using Telerik.UI.Xaml.Controls.Grid;

namespace SDKExamples.UWP.DataGrid
{
    public sealed partial class NestedPropertyName : ExamplePageBase
    {
        private DataGridTextColumn textColumn;
        public NestedPropertyName()
        {
            this.InitializeComponent();

            ObservableCollection<NestedPropertyNameitem> source = new ObservableCollection<NestedPropertyNameitem>();
            for (int i = 0; i < 100; i++)
            {
                source.Add(new NestedPropertyNameitem()
                {
                    Age = i * 10,
                    Name = i.ToString(),
                    Data = new Data()
                    {
                        Name = "Data " + i,
                        Information = new Info()
                        {
                            Value = i * 3,
                            Name = new Profile()
                            {
                                Name = "Profile " + i
                            }
                        }
                    }
                });
            }

            this.DataContext = source;
        }

        private void OnChangePropertyNameClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.nameColumn.PropertyName == "Name")
            {
                this.nameColumn.PropertyName = "Data.Information.Value";
            }
            else
            {
                this.nameColumn.PropertyName = "Name";
            }
        }

        private void AddRemoveColumnClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.textColumn == null)
            {
                this.textColumn = new DataGridTextColumn();
                this.textColumn.PropertyName = "Data.Information.Name.Name";
                this.textColumn.CanUserEdit = false;
                this.textColumn.CanUserFilter = false;
                this.textColumn.CanUserGroup = false;
                this.textColumn.CanUserReorder = false;
                this.textColumn.CanUserResize = false;
                this.textColumn.CanUserSort = false;
            }

            if (this.grid.Columns.Contains(this.textColumn))
            {
                this.grid.Columns.Remove(this.textColumn);
            }
            else
            {
                this.grid.Columns.Add(this.textColumn);
            }
        }
    }

    public class NestedPropertyNameitem
    {
        public string Name { get; set; }

        public double Age { get; set; }

        public Data Data { get; set; }
    }

    public class Data
    {
        public string Name { get; set; }

        public Info Information { get; set; }
    }

    public class Info
    {
        public int Value { get; set; }

        public Profile Name { get; set; }
    }

    public class Profile
    {
        public string Name { get; set; }
    }
}
