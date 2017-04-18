using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    public class ExtendedOrder : Order
    {
        public bool HasShipped { get; set; }

        public ExtendedOrder(Order sourceOrder)
        {
            this.Advertisement = sourceOrder.Advertisement;
            this.Date = sourceOrder.Date;
            this.Net = sourceOrder.Net;
            this.Product = sourceOrder.Product;
            this.Promotion = sourceOrder.Promotion;
            this.Quantity = sourceOrder.Quantity;
        }

        public static IList<ExtendedOrder> GetExtendedData()
        {
            var data = Order.GetData();
            var extendedData = new List<ExtendedOrder>();

            for (int i = 0; i < data.Count; i++)
            {
                var newOrder = new ExtendedOrder(data[i]);
                newOrder.HasShipped = (i % 2)  == 0;

                extendedData.Add(newOrder);
            }

            return extendedData;
        }
    }
}
