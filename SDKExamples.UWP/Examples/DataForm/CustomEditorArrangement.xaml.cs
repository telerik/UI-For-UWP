using System;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Data;
using Telerik.UI.Xaml.Controls.Data.DataForm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.DataForm
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomEditorArrangement : ExamplePageBase
    {
        public CustomEditorArrangement()
        {
            this.InitializeComponent();

            this.DataContext = new Item() { BirthDate = DateTime.Now, Age = 0, Name = "Ivaylo", PhoneNumber = "1234567890" };
        }

        public class Item
        {
            [Display(Header = "Name")]
            public string Name { get; set; }
            [Display(Header = "PhoneNumber")]
            public string PhoneNumber { get; set; }
            [Display(Header = "Age")]
            public double Age { get; set; }
            [Display(Header = "BirthDate")]
            public DateTime BirthDate { get; set; }
        }

    }

    public class CustomDataFormLayoutDefinition : StackDataFormLayoutDefinition
    {
        protected override Panel CreateDataFormPanel()
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            return grid;
        }

        protected override void SetEditorArrangeMetadata(EntityPropertyControl editorElement, EntityProperty entityProperty, Panel parentPanel)
        {
            editorElement.VerticalAlignment = VerticalAlignment.Center;
            editorElement.Margin = new Thickness(10);
            double column = 0;
            if (entityProperty.PropertyName.Equals("Name"))
            {
                column = 3;
            }
            else if (entityProperty.PropertyName.Equals("PhoneNumber"))
            {
                column = 2;
            }
            else if (entityProperty.PropertyName.Equals("Age"))
            {
                column = 1;
            }
            else
            {
                column = 0;
            }

            editorElement.SetValue(Grid.ColumnProperty, column);
        }
    }
}
