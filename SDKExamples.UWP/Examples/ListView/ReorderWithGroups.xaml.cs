using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;

namespace SDKExamples.UWP.Listview
{
    public sealed partial class ReorderWithGroups : ExamplePageBase
    {
        public ReorderWithGroups()
        {
            this.InitializeComponent();
            this.DataContext = new ViewModel();
        }

        public class ViewModel
        {
            public ObservableCollection<Item> Items { get; }

            public ViewModel()
            {
                this.Items = new ObservableCollection<Item>();

                for (int index = 0; index < 100; index++)
                {
                    var item = new Item
                    {
                        Category = $"Category {index / 10}",
                        Name = $"Item {index}"
                    };

                    this.Items.Add(item);
                }
            }
        }

        public class Item : INotifyPropertyChanged
        {
            private string category;
            private string name;

            public string Category
            {
                get
                {
                    return this.category;
                }
                set
                {
                    if (this.category != value)
                    {
                        this.category = value;
                        this.OnPropertyChanged();
                    }
                }
            }

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
                        this.OnPropertyChanged();
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class ListViewGroupReorderCommand : ListViewCommand
    {
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var items = (IList<ReorderWithGroups.Item>)this.Owner.ItemsSource;
            var context = (ItemReorderCompleteContext)parameter;
            var sourceItem = (ReorderWithGroups.Item)context.Item;

            items.Remove(sourceItem);

            var destinationItem = (ReorderWithGroups.Item)context.DestinationItem;
            var destinationGroup = context.DestinationGroup;
            var destinationIndex = items.IndexOf(destinationItem);

            if (context.Placement == ItemPlacement.After)
            {
                destinationIndex++;
            }

            sourceItem.Category = (string)destinationGroup.Key;
            items.Insert(destinationIndex, sourceItem);
        }
    }
}
