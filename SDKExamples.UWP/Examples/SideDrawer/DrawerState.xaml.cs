using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.SideDrawer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DrawerState : ExamplePageBase
    {
        public DrawerState()
        {
            this.InitializeComponent();
        }

        private void ShowOrHideDrawer(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (this.sideDrawer.DrawerState == Telerik.UI.Xaml.Controls.Primitives.DrawerState.Closed)
            {
                this.sideDrawer.ShowDrawer();
            }
            else
            {
                this.sideDrawer.HideDrawer();
            }
        }
    }
}
