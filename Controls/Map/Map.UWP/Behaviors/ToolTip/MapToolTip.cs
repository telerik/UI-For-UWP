using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Map.Primitives
{
    /// <summary>
    /// This class represents the toolTips for <see cref="RadMap"/>.
    /// </summary>
    [TemplateVisualState(Name = "Normal")]
    [TemplateVisualState(Name = "BottomRightAligned")]
    [TemplateVisualState(Name = "TopLeftAligned")]
    [TemplateVisualState(Name = "TopRightAligned")]
    public class MapToolTip : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapToolTip"/> class.
        /// </summary>
        public MapToolTip()
        {
            this.DefaultStyleKey = typeof(MapToolTip);
        }

        internal void UpdateVisualState(bool horizontalFlip, bool verticalFlip)
        {
            string state = "Normal";
            if (horizontalFlip && verticalFlip)
            {
                state = "TopRightAligned";
            }
            else if (horizontalFlip)
            {
                state = "BottomRightAligned";
            }
            else if (verticalFlip)
            {
                state = "TopLeftAligned";
            }

            VisualStateManager.GoToState(this, state, false);
        }
    }
}
