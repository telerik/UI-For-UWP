using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Expander
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Customizations : ExamplePageBase
    {
        public Customizations()
        {
            this.InitializeComponent();
        }

        private void expander_ExpandedStateChanged(object sender, ExpandedStateChangedEventArgs e)
        {
            if (textblock == null)
            {
                return;
            }
            this.textblock.Text = e.IsExpanded ? "The control is expanded" : "The control is collapsed";
        }
    }
}
