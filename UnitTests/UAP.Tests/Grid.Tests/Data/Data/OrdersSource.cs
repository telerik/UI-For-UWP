using System.Collections.ObjectModel;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    public class OrdersSource : ReadOnlyCollection<Order>
    {
        public OrdersSource()
            : base(Order.GetData())
        {
        }
    }
}
