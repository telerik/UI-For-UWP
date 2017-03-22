using System;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Grid;

namespace SDKExamples.UWP.DataGrid
{
    public sealed partial class EditMode : ExamplePageBase
    {
        private static Random rand = new Random(0);

        public EditMode()
        {
            this.InitializeComponent();
            this.DataContext = new ObservableCollection<Item>(Enumerable.Range(0, 10).Select(i => new Item { Age = rand.Next(15, 30), Name = "Customer " + i }));
        }

        public class Item : ViewModelBase

        {
            private string name;
            private int age;

            public string Name
            {
                get { return name; }
                set
                {
                    if (name != value)
                    {
                        name = value;
                        OnPropertyChanged();
                    }
                }
            }

            public int Age

            {
                get { return age; }
                set
                {
                    if (age != value)
                    {
                        age = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.grid.UserEditMode == DataGridUserEditMode.External)
            {
                this.grid.UserEditMode = DataGridUserEditMode.Inline;
            }
            else
            {
                this.grid.UserEditMode = DataGridUserEditMode.External;
            }
        }
    }
}
