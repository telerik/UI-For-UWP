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
    public sealed partial class FrozenColumnsExample : ExamplePageBase
    {
        public FrozenColumnsExample()
        {
            this.InitializeComponent();

            ObservableCollection<Item> source = new ObservableCollection<Item>();
            source.Add(new Item { Name = "Ivaylo", Age = 25, Birthday = DateTime.Today.AddYears(-25) });
            source.Add(new Item { Name = "Rosi", Age = 27, Birthday = DateTime.Today.AddYears(-27) });
            source.Add(new Item { Name = "Ivan", Age = 26, Birthday = DateTime.Today.AddYears(-26) });
            source.Add(new Item { Name = "Kaloyan", Age = 30, Birthday = DateTime.Today.AddYears(-30) });

            this.DataContext = source;


            for (int i = 0; i < 10; i++)
            {
                this.grid.Columns.Add(new DataGridTextColumn { PropertyName = "Name" });
                this.grid.Columns.Add(new DataGridTextColumn { PropertyName = "Age" });
                this.grid.Columns.Add(new DataGridTextColumn { PropertyName = "Birthday" });
            }
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

            public bool IsMarried
            {
                get
                {
                    return isMarried;
                }
                set
                {
                    if (this.isMarried != value)
                    {
                        this.isMarried = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            this.grid.FrozenColumnCount = (int)e.NewValue;
        }
    }
}
