using System;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Event arguments for RadPictureHubTile's PictureNeeded event.
    /// </summary>
    public class PictureNeededEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the source of the picture.
        /// </summary>
        public ImageSource Source
        {
            get;
            set;
        }
    }
}
