using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.AutoCompleteBox
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BindingComplexObjects : ExamplePageBase
    {
        public BindingComplexObjects()
        {
            this.InitializeComponent();

            List<Street> source = new List<Street>();
            source.Add(new Street() { City = "Vratsa", Country = "Bulgaria", Name = "Iliya Krustenyakov", State = "Vratsa" });
            source.Add(new Street() { City = "Sofia", Country = "Bulgaria", Name = "Zagore", State = "Sofia" });
            source.Add(new Street() { City = "Pleven", Country = "Bulgaria", Name = "Botev", State = "Pleven" });
            source.Add(new Street() { City = "Byala Slatina", Country = "Bulgaria", Name = "Hristo Botev", State = "Pleven" });

            this.autoComplete.ItemsSource = source;

            // You can also use a provider.
            //this.autoComplete.FilterMemberProvider = (object item) =>
            //{
            //    return (item as Street).Name + " St.";
            //};
        }

        public class Street
        {
            public string Name
            {
                get;
                set;
            }

            public string State
            {
                get;
                set;
            }

            public string Country
            {
                get;
                set;
            }

            public string City
            {
                get;
                set;
            }
        }

    }
}
