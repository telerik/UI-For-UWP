using System;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.HubTile
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HexHubTile : ExamplePageBase
    {
        public HexHubTile()
        {
            this.InitializeComponent();

            this.orientationCombo.ItemsSource = Enum.GetValues(typeof(HexOrientation));
            this.orientationCombo.SelectedIndex = 0;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.hubtile.Orientation = (HexOrientation)(sender as ComboBox).SelectedItem;
        }
    }
}
