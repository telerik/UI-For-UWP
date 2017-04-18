using Telerik.Data.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.DataForm
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Grouping : ExamplePageBase
    {
        public Grouping()
        {
            this.InitializeComponent();

            this.DataContext = new Item() { Age = 19, Language = Language.German, Married = true, Name = "Ivaylo" };
        }

        public class Item
        {
            [Display(Header = "Name")]
            public string Name { get; set; }
            [Display(Header = "Age")]
            public double Age { get; set; }

            [Display(Group = "Additional Information", Header = "Language")]
            public Language Language { get; set; }
            [Display(Group = "Additional Information", Header = "Married")]
            public bool Married { get; set; }    
        }
        public enum Language
        {
            Bulgarian,
            English,
            German,
        }
    }

    public class DataFormGroupHeaderSelector : DataTemplateSelector
    {
        public DataTemplate HeaderTemplate { get; set; }
        protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        {
            if (item.ToString() == "Additional Information")
            {
                return this.HeaderTemplate;
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}
