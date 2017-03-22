using System.Collections.ObjectModel;
using System.Linq;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Listview
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Reorder : ExamplePageBase
    {
        public Reorder()
        {
            this.InitializeComponent();
            this.DataContext = new ObservableCollection<Item>(Enumerable.Range(0, 10).Select(x => new Item { Id = x, Name = "Mr. " + x }));
        }

        public class Item
        {
            public string Name { get; set; }
            public double Id { get; set; }
        }
    }

    public class ListViewItemReorderCommand : ListViewCommand
    {
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var source = (this.Owner.ItemsSource as ObservableCollection<Reorder.Item>);
            var context = parameter as ItemReorderCompleteContext;
            var itemIndex = source.IndexOf(context.Item as Reorder.Item);
            var destinationItemIndex = source.IndexOf(context.DestinationItem as Reorder.Item);

            source.Move(itemIndex, destinationItemIndex);

            this.Owner.CommandService.ExecuteDefaultCommand(CommandId.ItemReorderComplete, parameter);
        }
    }
}
