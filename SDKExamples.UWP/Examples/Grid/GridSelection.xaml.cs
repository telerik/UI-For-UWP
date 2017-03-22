using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.DataGrid
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GridSelection : ExamplePageBase
    {
        public GridSelection()
        {
            this.InitializeComponent();

            ObservableCollection<Item> source = new ObservableCollection<Item>();
            source.Add(new Item { Name = "Ivaylo", Age = 25, Birthday = DateTime.Today.AddYears(-25) });
            source.Add(new Item { Name = "Rosi", Age = 27, Birthday = DateTime.Today.AddYears(-27) });
            source.Add(new Item { Name = "Ivan", Age = 26, Birthday = DateTime.Today.AddYears(-26) });
            source.Add(new Item { Name = "Kaloyan", Age = 30, Birthday = DateTime.Today.AddYears(-30) });

            this.DataContext = source;
            this.comboMode.ItemsSource = Enum.GetValues(typeof(DataGridSelectionMode));
            this.comboMode.SelectedItem = DataGridSelectionMode.Single;

            this.comboUnit.ItemsSource = Enum.GetValues(typeof(DataGridSelectionUnit));
            this.comboUnit.SelectedItem = DataGridSelectionUnit.Row;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.grid.SelectionMode = (DataGridSelectionMode)(sender as ComboBox).SelectedItem;
        }

        private void ComboBoxMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.grid.SelectionUnit = (DataGridSelectionUnit)(sender as ComboBox).SelectedItem;
        }



        public class Item : ViewModelBase
        {
            private string name;
            private int age;
            private DateTime birthday;
            private bool isMarried;

            public string Name
            {
                get
                {
                    return this.name;
                }
                set
                {
                    if (this.name != value)
                    {
                        this.name = value;
                        OnPropertyChanged();
                    }
                }
            }

            public int Age
            {
                get
                {
                    return age;
                }
                set
                {
                    if (this.age != value)
                    {
                        this.age = value;
                        OnPropertyChanged();
                    }
                }
            }

            public DateTime Birthday
            {
                get
                {
                    return birthday;
                }
                set
                {
                    if (this.birthday != value)
                    {
                        this.birthday = value;
                        OnPropertyChanged();
                    }
                }
            }
        }
    }
}
