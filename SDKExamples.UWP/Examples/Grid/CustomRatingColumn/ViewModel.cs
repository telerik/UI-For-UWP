using System;
using System.Collections.ObjectModel;

namespace SDKExamples.UWP.DataGrid
{
    public class ViewModel
    {
        public ViewModel()
        {
            this.Items = ViewModel.GenerateItems();
        }

        public ObservableCollection<Item> Items { get; set; }

        private static ObservableCollection<Item> GenerateItems()
        {
            var randomRating = new Random();
            var items = new ObservableCollection<Item>();
            for (int i = 0; i < 25; i++)
            {
                var item = new Item() { Name = "Name " + i, Rating = randomRating.Next(1, 5) };
                items.Add(item);
            }

            return items;
        }
    }
}