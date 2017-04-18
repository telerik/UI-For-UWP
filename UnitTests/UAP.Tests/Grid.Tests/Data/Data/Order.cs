using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    public class Order
    {
        private static List<Order> loadedData;

        private int quantity;

        public DateTime Date { get; set; }
        public string Product { get; set; }
        public string Promotion { get; set; }
        public double Net { get; set; }

        public int Quantity
        {
            get
            {
                if (this.InvalidQuantity)
                {
                    throw new Exception("InvalidQuantity is set to true");
                }

                return this.quantity;
            }
            set
            {
                this.quantity = value;
            }
        }

        public object Advertisement { get; set; }

        /// <summary>
        /// Setting this to true will cause the get Promotion to throw an exception.
        /// </summary>
        public bool InvalidQuantity { get; set; }

        public override string ToString()
        {
            return this.Product + this.Promotion + this.Quantity;
        }

        public static IList<Order> GetData()
        {
            var data = LoadData();

            return data.ToList();
        }

        public static IList<Order> GetSmallData()
        {
            var data = LoadData();

            return data.Take(5).ToList();
        }

        private static IList<Order> LoadData()
        {
            if (loadedData == null)
            {
                loadedData = new List<Order>();
                var sri = typeof(Order).GetTypeInfo().Assembly.GetManifestResourceStream("Telerik.UI.Xaml.Controls.Grid.Tests.Data.Data.OrderData.txt");
                var streamReader = new StreamReader(sri);

                string file = streamReader.ReadToEnd();
                using (var reader = new StringReader(file))
                {
                    while (reader.Peek() != -1)
                    {
                        string[] items = reader.ReadLine().Split('\t');
                        Order o = new Order()
                        {
                            Date = DateTime.Parse(items[0], CultureInfo.InvariantCulture),
                            Product = items[1],
                            Quantity = int.Parse(items[2], CultureInfo.InvariantCulture),
                            Net = double.Parse(items[3], CultureInfo.InvariantCulture),
                            Promotion = items[4],
                            Advertisement = items[5]
                        };
                        loadedData.Add(o);
                    }
                }
            }

            return loadedData;
        }
    }

    public enum OrderFields
    {
        Date,
        Product,
        Quantity,
        Net,
        Promotion,
        Advertisement,
        Order
    }

    public static class OrderFieldsExtensions
    {
        public static object GetField(this OrderFields field, object item)
        {
            Order order = (Order)item;
            if (order != null)
            {
                switch (field)
                {
                    case OrderFields.Date: return order.Date;
                    case OrderFields.Product: return order.Product;
                    case OrderFields.Quantity: return order.Quantity;
                    case OrderFields.Net: return order.Net;
                    case OrderFields.Promotion: return order.Promotion;
                    case OrderFields.Advertisement: return order.Advertisement;
                    case OrderFields.Order: return order;
                }
            }
            throw new Exception(String.Format("Field {0} not found on order {1}.", field, order));
        }
    }
}