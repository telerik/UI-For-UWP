using System.Collections.Generic;
using Windows.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Listview
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScrollIndexIntoView : ExamplePageBase
    {
        public ScrollIndexIntoView()
        {
            this.InitializeComponent();

            var items = new List<string>();
            for (int i = 0; i < 1000; i++)
            {
                items.Add("Item " + i);
            }

            this.listView.ItemsSource = items;
            this.numericBox.Maximum = items.Count - 1;
        }

        private void ScrollIndexIntoViewClicked(object sender, RoutedEventArgs e)
        {
            if (this.numericBox.Value != null)
            {
                this.listView.ScrollIndexIntoView((int)this.numericBox.Value);
            }
        }
    }
}
