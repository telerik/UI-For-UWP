using System;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.HubTile
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PictureRotatorHubTile : ExamplePageBase
    {
        public const string RelativePath = "../../Images/";

        public PictureRotatorHubTile()
        {
            this.InitializeComponent();

            List<PicturePath> pictures = new List<PicturePath>()
            {
                new PicturePath{PictureName="donkey.jpg"},
                new PicturePath{PictureName="walnuts.jpg"},
                new PicturePath{PictureName="flowers.jpg"},
                new PicturePath{PictureName="butterfly.jpg"},
            };

            this.DataContext = pictures;

        }
    }
}
