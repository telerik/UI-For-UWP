using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.UI.Xaml.Data;

namespace SDKExamples.UWP.DataGrid
{
    public sealed partial class NestedPropertyName : ExamplePageBase
    {
        private static int newValue = 0;
        private DataGridTextColumn textColumn;
        private ObservableCollection<NestedPropertyNameitem> source;
        public NestedPropertyName()
        {
            this.InitializeComponent();

            this.source = new ObservableCollection<NestedPropertyNameitem>();
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

        private void CancelEditButtonClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.grid.CancelEdit();
        }

        private void OnUpdateOnPropertyChangedButtonClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.grid.ListenForNestedPropertyChange = !this.grid.ListenForNestedPropertyChange;
        }

        private void OnChangeFirstItemClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Info newInfo = new Info() { Name = new Profile() { Name = "New Info" }, Value = 2000 + newValue++ };
            this.source[0].Data.Information = newInfo;
            this.source[0].Data.Information.Value += newValue;
        }

        private void OnAddItemButtonClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var newItem = new NestedPropertyNameitem()
            {
                Age = 1000 + newValue,
                Name = "New Value",
                Data = new Data()
                {
                    Name = "NewData " + newValue,
                    Information = new Info()
                    {
                        Value = newValue * 3,
                        Name = new Profile()
                        {
                            Name = "NewProfile " + newValue
                        }
                    }
                }
            };

            this.source.Add(newItem);
        }

        private void OnRemoveFirstItemButtonClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.source.Count > 0)
            {
                this.source.RemoveAt(0);
            }
        }
    }

    public class NestedPropertyNameitem : ViewModelBase
    {
        private string name;
        private double age;
        private Data data;

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
                    this.OnPropertyChanged(nameof(Name));
                }
            }
        }

        public double Age
        {
            get
            {
                return this.age;
            }
            set
            {
                if (this.age != value)
                {
                    this.age = value;
                    this.OnPropertyChanged(nameof(Age));
                }
            }
        }

        public Data Data
        {
            get
            {
                return this.data;
            }
            set
            {
                if (this.data != value)
                {
                    this.data = value;
                    this.OnPropertyChanged(nameof(Data));
                }
            }
        }
    }

    public class Data : ViewModelBase
    {
        private string name;
        private Info information;

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
                    this.OnPropertyChanged(nameof(Name));
                }
            }
        }

        public Info Information
        {
            get
            {
                return this.information;
            }
            set
            {
                if (this.information != value)
                {
                    this.information = value;
                    this.OnPropertyChanged(nameof(Information));
                }
            }
        }
    }

    public class Info : ViewModelBase
    {
        private int value;
        private Profile name;

        public int Value
        {
            get
            {
                return this.value;
            }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    this.OnPropertyChanged(nameof(Value));
                }
            }
        }

        public Profile Name
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
                    this.OnPropertyChanged(nameof(Name));
                }
            }
        }
    }

    public class Profile : ViewModelBase
    {
        private string name;
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
                    this.OnPropertyChanged(nameof(Name));
                }
            }
        }
    }
}
