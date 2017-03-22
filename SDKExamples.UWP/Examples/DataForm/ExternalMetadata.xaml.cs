using System;
using System.Reflection;
using Telerik.Data.Core;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.DataForm
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExternalMetadata : ExamplePageBase
    {
        public ExternalMetadata()
        {
            this.InitializeComponent();

            this.DataContext = new Item() { PhoneNumber = "1234567890", Date = DateTime.Now };
        }

        public class Item
        {
            public string PhoneNumber { get; set; }
            public DateTime Date { get; set; }
        }
    }

    public class ExternalMetadataProvider : RuntimeEntityProvider
    {
        protected override Type GetEntityPropertyType(object property)
        {
            return typeof(UserEntityProperty);
        }
    }

    public class UserEntityProperty : RuntimeEntityProperty
    {
        public UserEntityProperty(PropertyInfo property, object item)
        : base(property, item)
        {
        }

        protected override string GetLabel(object property)
        {
            var prop = property as PropertyInfo;
            return "Custom label for " + prop.Name;
        }
    }

}
