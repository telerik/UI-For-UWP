using Telerik.UI.Xaml.Controls.Data.DataForm;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.DataForm
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomEditor : ExamplePageBase
    {
        public CustomEditor()
        {
            this.InitializeComponent();

            this.DataContext = new Item() { ProductName = "Potatoes", Price = 10 };

            this.DataForm.RegisterPropertyEditor("ProductName", typeof(CustomStringEditorDF)); 
        }

        public class Item
        {
            public string ProductName { get; set; }
            public double Price { get; set; }
        }
    }

    public class CustomStringEditorDF : Control, ITypeEditor
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CustomStringEditorDF), new PropertyMetadata(null));


        public void BindEditor()
        {
            Binding b = new Binding();
            b.Mode = BindingMode.TwoWay;
            b.Path = new PropertyPath("PropertyValue");
            this.SetBinding(CustomStringEditorDF.TextProperty, b);
        }
    }
}
