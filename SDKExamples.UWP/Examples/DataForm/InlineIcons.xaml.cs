using System;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.DataForm
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InlineIcons : ExamplePageBase
    {
        public InlineIcons()
        {
            this.InitializeComponent();

            this.DataContext = new Item() {PhoneNumber = "1234567890", Date = DateTime.Now };
        }

        public class Item
        {
            public string PhoneNumber { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
