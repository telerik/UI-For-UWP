using System;
using Telerik.Data.Core;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.DataForm
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Attributes : ExamplePageBase 
    {
        public Attributes()
        {
            this.InitializeComponent();

            this.DataContext = new Item() { Arrival = DateTime.Now };
        }

        public class Item
        {
            [Display(Header = "Name", PlaceholderText = "Fill the Name of the passenger")]
            public string Name { get; set; }

            [Display(Header = "Age")]
            public double Age { get; set; }

            [Display(Header = "Arrive")]
            public DateTime Arrival { get; set; }
            
            [Display(Header = "Married", Group = "Additional Information")]
            [ReadOnly]
            public bool IsMarried { get; set; }
        }
    }
}
