using System.Collections.Generic;

namespace SDKExamples.UWP.Listview
{
    public sealed partial class HeaderFooter : ExamplePageBase
    {
        public HeaderFooter()
        {
            this.InitializeComponent();

            var items = new List<string>();

            for (int index = 0; index < 100; index++)
            {
                var item = $"Item {index}";

                items.Add(item);
            }

            this.listView.ItemsSource = items;
        }
    }
}
