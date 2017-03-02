using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Listview
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Layouts : ExamplePageBase
    {
        public Layouts()
        {
            this.InitializeComponent();
            this.DataContext = Enumerable.Range(0, 100);
            this.layoutMode.ItemsSource = new List<string>() { "Stack", "Wrap", "Staggered" };
            this.layoutMode.SelectedIndex = 0;

            this.orientationMode.ItemsSource = Enum.GetNames(typeof(Orientation));
            this.orientationMode.SelectedIndex = 0;
        }

        private void LayoutModeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string definitionString = (sender as ComboBox).SelectedItem.ToString();

            LayoutDefinitionBase layout = null;

            switch (definitionString)
            {
                case "Stack":
                    layout = new StackLayoutDefinition();
                   this.listView.ItemTemplate = Root.Resources["DefaultTemplate"] as DataTemplate;
                    break;
                case "Wrap":
                    layout = new WrapLayoutDefinition() { ItemWidth = 400};
                    this.listView.ItemTemplate = Root.Resources["DefaultTemplate"] as DataTemplate;
                    break;
                case "Staggered":
                    layout = new StaggeredLayoutDefinition();
                    this.listView.ItemTemplate = null;
                    break;
                default:
                    break;
            }
            this.listView.ItemTemplateSelector = Root.Resources["Selector"] as DataTemplateSelector;

            this.listView.LayoutDefinition = layout;
        }

        private void OrientationModeChanged(object sender, SelectionChangedEventArgs e)
        {
            this.listView.Orientation = (Orientation)Enum.Parse(typeof(Orientation), (string)(sender as ComboBox).SelectedItem);
        }
    }

    public class StaggeredTemplateSelector:DataTemplateSelector
    {
        public DataTemplate Template1 { get; set; }
        public DataTemplate Template2 { get; set; }
        public DataTemplate Template3 { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var index = (int)item;

            if (index % 3 == 0)
            {
                return Template1;
            }

            if (index % 5 == 0)
            {
                return Template2;
            }

            return Template3;
        }
    }
}
