using System;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.HubTile
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MosaicHubTile : ExamplePageBase
    {
        public MosaicHubTile()
        {
            this.InitializeComponent();

            this.flipModeCombo.ItemsSource = Enum.GetValues(typeof(MosaicFlipMode));
            this.flipModeCombo.SelectedIndex = 0;

            this.DataContext = new List<PicturePath>()
            {
                new PicturePath { PictureName = "donkey.jpg" },
                new PicturePath { PictureName = "walnuts.jpg" },
                new PicturePath { PictureName = "flowers.jpg" },
                new PicturePath { PictureName = "butterfly.jpg" },
            };
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.hubtile.FlipMode = (MosaicFlipMode)(sender as ComboBox).SelectedItem;
        }
    }
}
