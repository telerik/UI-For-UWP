using System;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.SideDrawer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Animations : ExamplePageBase
    {
        public Animations()
        {
            this.InitializeComponent();

            this.DataContext = Enum.GetValues(typeof(DrawerTransition));
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.sideDrawer.DrawerTransition = (DrawerTransition)(sender as ListView).SelectedItem;
        }
    }
}
